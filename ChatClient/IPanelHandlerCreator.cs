
namespace ChatClient
{
    /// <summary>
    /// Interface providing a method to create panelHandlers.
    /// </summary>
    public interface IPanelHandlerCreator
    {
        /// <summary>
        /// Creates panel handlers.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>IPanelHandler with strategy of type dependant on parameter.</returns>
        IPanelHandler createPanelHandler(int type);
    }
}

/*
Abstract part of factory method pattern.
Analogus in compliance with solid to factory method pattern in ChatServer.
*/