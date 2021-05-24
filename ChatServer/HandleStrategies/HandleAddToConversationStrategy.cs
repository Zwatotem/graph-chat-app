using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
    /// <summary>
    /// Class handling request to add user to conversation.
    /// </summary>
    class HandleAddToConversationStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add user to conversation");
            //decoding request - first 4 bytes are id of conversation to which the user is to be added, following bytes are user name
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            string nameToAdd = Encoding.UTF8.GetString(messageBytes, 4, messageBytes.Length - 4);
            Console.WriteLine("DEBUG: trying to add user to conversation");
            byte[] reply = new byte[1]; //boolean reply has only 1 byte
            lock (allHandlers) //prohibit other threads from interfering
            {
                if (chatSystem.addUserToConversation(nameToAdd, conversationId))
                {
                    reply[0] = 1; //if adding successful set reply byte to one
                    byte[] msg = messageBytes;
                    Conversation conversation = chatSystem.getConversation(conversationId);
                    //and broadcast the change to all active handlers handling users present in the conversation
                    foreach (var handler in allHandlers.FindAll(h => conversation.Users.Exists(u => u.Name == h.HandledUserName)))
                    {
                        if (handler.HandledUserName == nameToAdd) //newly added user has to receive the entire conversation
                        {
                            byte[] update = conversation.serialize(new ConcreteSerializer()).ToArray();
                            handler.sendMessage(5, update); //serialized conversation - type 5
                        }
                        else
                        {
                            handler.sendMessage(4, msg); //user added to conversation - type 4. Forwarding received request.
                        }
                    }
                }
                else
                {
                    reply[0] = 0; //else set reply byte to one
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
