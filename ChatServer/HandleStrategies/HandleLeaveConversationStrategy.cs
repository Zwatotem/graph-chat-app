using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    /// <summary>
    /// Class handling request to leave conversation.
    /// </summary>
    class HandleLeaveConversationStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "leave conversation");
            //decoding request - first 4 bytes are id of conversation which the user wants to leave
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            string userName = handlerThread.HandledUserName;
            Console.WriteLine("DEBUG: trying to remove user from conversation");
            byte[] reply = new byte[1]; //boolean reply has only 1 byte
            lock (allHandlers) //prohibit other threads from interfering
            {
                if (chatSystem.leaveConversation(userName, conversationId))
                {
                    reply[0] = 1;
                    int messageLength = 4 + Encoding.UTF8.GetByteCount(userName);
                    byte[] msg = new byte[messageLength];
                    //message to be broadcasted - 4 bytes are id of conversation, the rest are user name
                    Array.Copy(messageBytes, 0, msg, 0, 4);
                    Array.Copy(Encoding.UTF8.GetBytes(userName), 0, msg, 4, messageLength - 4);
                    Conversation conversation = chatSystem.getConversation(conversationId);
                    if (conversation != null) //if there are users left in the conversation
                    {
                        //notify other clients of the change
                        foreach (var handler in allHandlers.FindAll(h => conversation.Users.Exists(u => u.Name == h.HandledUserName)))
                        {
                            handler.sendMessage(3, msg); //left conversation - type 3
                        }
                    }
                }
                else
                {
                    reply[0] = 0;
                }
            }
            handlerThread.sendMessage(1, reply); //send the reply
        }
    }
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/