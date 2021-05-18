using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    public interface IHandleRequestStrategy
    {
        void handleMessage(ChatServer chatServer, ChatSystem chatSystem, HandlerThread handlerThread, byte[] messageBytes);
    }
}
