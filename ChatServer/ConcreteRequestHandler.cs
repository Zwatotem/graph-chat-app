using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public class ConcreteRequestHandler : IRequestHandler
    {
        private IHandleStrategy handleStrategy;

        public ConcreteRequestHandler(IHandleStrategy handleStrategy)
        {
            this.handleStrategy = handleStrategy;
        }

        public void handleRequest(List<IClientHandler> allHandlers, IChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            this.handleStrategy.handleRequest(allHandlers, chatSystem, handlerThread, messageBytes);
        }
    }
}
