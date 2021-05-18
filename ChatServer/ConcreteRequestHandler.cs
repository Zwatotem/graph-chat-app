using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, ClientHandler handlerThread, byte[] messageBytes)
        {
            this.handleStrategy.handleMessage(chatServer, chatSystem, handlerThread, messageBytes);
        }
    }
}
