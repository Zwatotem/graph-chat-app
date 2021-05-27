
namespace ChatClient
{
    /// <summary>
    /// Interface generalizing the strategy to handle a transmission from server.
    /// </summary>
    public interface IHandleTransmissionStrategy
    {
        /// <summary>
        /// Handles a transmission received from server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="inBuffer"></param>
        void handle(ChatClient client, byte[] inBuffer);
    }
}

/*
Part of strategy pattern, implemented by classe in the HandleTransmissionStrategies folder.
SOLID and so on analogus to strategy pattern in ChatServer.
*/
