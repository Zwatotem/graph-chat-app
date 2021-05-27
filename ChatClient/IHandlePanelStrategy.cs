
namespace ChatClient
{
    /// <summary>
    /// Interface generalizing the strategy to handle a panel.
    /// </summary>
    public interface IHandlePanelStrategy
    {
        /// <summary>
        /// Displays and handles panel.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>int indicating the next panel to handle.</returns>
        int handle(ChatClient client);
    }
}

/*
Part of strategy pattern, implemented by classe in the HandlePanelStrategies folder.
SOLID and so on analogus to strategy pattern in ChatServer.
*/
