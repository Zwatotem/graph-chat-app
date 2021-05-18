using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    class HandleUnknownRequest : IHandleRequestStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, HandlerThread handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: unknown request receive");
        }
    }
}
