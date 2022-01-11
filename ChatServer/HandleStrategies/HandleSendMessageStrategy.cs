using System;
using System.Collections.Generic;
using System.Linq;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
    class HandleSendMessageStrategy : IHandleStrategy
    {
        /// <summary>
        /// Class handling request to send message to conversation.
        /// </summary>
        public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "send message");
            //decoding request - first 4 bytes are id of conversation, next 4 are id of message to which we reply the rest are message content bytes
            Guid conversationId = new Guid(messageBytes[0..16]);
            Guid targetedMessageId = new Guid(messageBytes[16..32]);
            IMessageContentCreator contentCreator = new ConcreteMessageContentCreator(); //creating instance of factory pattern
            
            IMessageContent content = contentCreator.createMessageContent(messageBytes, 32); //utilizing factory pattern to create message content
            //it allows for dependency inversion, as here we only have content with creator interface and we can choose any concrete creator
            //it also allows this class not to contain the logic necessary for creating message content (single responsibility)
            byte[] reply = new byte[1];
            if (content == null)
            {
                reply[0] = 0; //if valid content could not be created from given bytes indicate failure
            }
            else
            {
                Console.WriteLine("DEBUG: trying to send message");
                lock (allHandlers)
                {
                    Message sentMessage = chatSystem.SendMessage(conversationId, handlerThread.HandledUserName, targetedMessageId, content, DateTime.Now);
                    if (sentMessage != null)
                    {
                        //if successful conversation id with serialized message are broadcasted to all connected users from this conversation
                        reply[0] = 1;
                        byte[] serializedBytes = sentMessage.serialize(new ConcreteSerializer()).ToArray();
                        byte[] msg = new byte[serializedBytes.Length + 16];
                        Array.Copy(conversationId.ToByteArray(), 0, msg, 0, 16);
                        Array.Copy(serializedBytes, 0, msg, 16, serializedBytes.Length);
                        Conversation conversation = chatSystem.GetConversation(conversationId);
                        foreach (var handler in allHandlers.FindAll(h => conversation.Users.Any(u => u.Name == h.HandledUserName)))
                        {
                            handler.sendMessage(6, msg); //sent message - type 6
                        }
                    }
                    else
                    {
                        reply[0] = 0;
                    }
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/