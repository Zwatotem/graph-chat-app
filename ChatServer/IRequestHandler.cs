using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    public interface IRequestHandler
    {
        void handleMessage(ChatServer chatServer, ChatSystem chatSystem, ClientHandler handlerThread, byte[] messageBytes);
    }
}
