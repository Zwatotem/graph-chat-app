
namespace ChatClient
{
    /// <summary>
    /// Interface providing method to handle transmission from server.
    /// </summary>
    public interface ITransmissionHandler
    {
        /// <summary>
        /// Handles incoming transmission from server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="inBuffer"></param>
        void handle(ChatClient client, byte[] inBuffer);
    }
}

/*
Strategy pattern context. 
Analogus in compliance with solid to strategy pattern in ChatServer.
*/