using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleLoginStrategy : IHandleStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, ClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "logIn");
            string userName = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("DEBUG: requested logIn");
            byte[] reply = new byte[1];
            lock (chatServer)
            {
                User user = chatSystem.getUser(userName);
                if (handlerThread.HandledUserName != null || user == null || chatServer.Handlers.Exists(h => h.HandledUserName == userName))
                {
                    reply[0] = 0;
                }
                else
                {
                    reply[0] = 1;
                    handlerThread.HandledUserName = userName;
                    foreach (var conversation in user.getConversations())
                    {
                        byte[] msg = conversation.serialize().ToArray();
                        handlerThread.sendMessage(5, msg);
                    }
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}
