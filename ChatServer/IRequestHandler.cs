using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public interface IRequestHandler
    {
        void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes);
    }
}
