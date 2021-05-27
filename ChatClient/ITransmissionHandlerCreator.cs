
namespace ChatClient
{
    /// <summary>
    /// Interface providing a method to create transmission handling strategy context.
    /// </summary>
    public interface ITransmissionHandlerCreator
    {
        /// <summary>
        /// Creates transmissionHandler.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>ITransmissionHandler with strategy field dependant on the parameter.</returns>
        ITransmissionHandler createTransmissionHandler(int type);
    }
}

/*
Abstract part of factory method pattern.
Analogus in compliance with solid to factory method pattern in ChatServer.
*/