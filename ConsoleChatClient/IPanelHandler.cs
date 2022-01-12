
namespace ChatClient
{
    /// <summary>
    /// Interface providing method to display panel.
    /// </summary>
    public interface IPanelHandler
    {
        /// <summary>
        /// Displays the proper panel and behaves accordingly depending on strategy withing the concrete object.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>int indicating next panel to display.</returns>
        int handle(ChatClient client);
    }
}

/*
Strategy pattern context. 
Analogus in compliance with solid to strategy pattern in ChatServer.
*/