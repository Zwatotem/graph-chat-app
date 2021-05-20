using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
    class HandleLoginStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "logIn");
            string userName = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("DEBUG: requested logIn");
            byte[] reply = new byte[1];
            lock (allHandlers)
            {
                User user = chatSystem.getUser(userName);
                if (handlerThread.HandledUserName != null || user == null || allHandlers.Exists(h => h.HandledUserName == userName))
                {
                    reply[0] = 0;
                }
                else
                {
                    reply[0] = 1;
                    handlerThread.HandledUserName = userName;
                    foreach (var conversation in user.getConversations())
                    {
                        byte[] msg = conversation.serialize(new ConcreteSerializer()).ToArray();
                        handlerThread.sendMessage(5, msg);
                    }
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}
