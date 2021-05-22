using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public interface IHandleStrategy
    {
        void handleRequest(List<IClientHandler> allHandlers, IChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes);
    }
}
