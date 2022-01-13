
namespace ChatClient
{
    /// <summary>
    /// Concrete implementation of ITransmissionHandler.
    /// </summary>
    public class ConcreteTransmissionHandler : ITransmissionHandler
    {
        private IHandleTransmissionStrategy handleStrategy;

        public ConcreteTransmissionHandler(IHandleTransmissionStrategy handleStrategy)
        {
            this.handleStrategy = handleStrategy;
        }

        public void handle(ChatClient client, byte[] inBuffer)
        {
            handleStrategy.handle(client, inBuffer);
        }
    }
}

/*
Strategy pattern context. 
Analogus in compliance with solid to strategy pattern in ChatServer.
*/