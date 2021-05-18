using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleUnknownStrategy : IHandleStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, ClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: unknown request receive");
        }
    }
}
