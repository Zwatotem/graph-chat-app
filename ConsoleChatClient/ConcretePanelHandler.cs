
namespace ChatClient
{
    /// <summary>
    /// Concrete implementation of IPanelHandler.
    /// </summary>
    public class ConcretePanelHandler : IPanelHandler
    {
        private IHandlePanelStrategy handleStrategy;

        public ConcretePanelHandler(IHandlePanelStrategy handleStrategy)
        {
            this.handleStrategy = handleStrategy;
        }

        public int handle(ChatClient client)
        {
            return handleStrategy.handle(client);
        }
    }
}

/*
Strategy pattern context. 
Analogus in compliance with solid to strategy pattern in ChatServer.
*/