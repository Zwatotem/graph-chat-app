using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleLeaveConversationStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "leave conversation");
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            string userName = handlerThread.HandledUserName;
            Console.WriteLine("DEBUG: trying to remove user from conversation");
            byte[] reply = new byte[1];
            lock (allHandlers)
            {
                if (chatSystem.leaveConversation(userName, conversationId))
                {
                    reply[0] = 1;
                    int messageLength = 4 + Encoding.UTF8.GetByteCount(userName);
                    byte[] msg = new byte[messageLength];
                    Array.Copy(messageBytes, 0, msg, 0, 4);
                    Array.Copy(Encoding.UTF8.GetBytes(userName), 0, msg, 4, messageLength - 4);
                    Conversation conversation = chatSystem.getConversation(conversationId);
                    foreach (var handler in allHandlers.FindAll(h => conversation.Users.Exists(u => u.getName() == h.HandledUserName)))
                    {
                        handler.sendMessage(3, msg);
                    }
                }
                else
                {
                    reply[0] = 0;
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}
