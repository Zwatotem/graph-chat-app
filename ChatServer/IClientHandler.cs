
namespace ChatServer;

/// <summary>
/// Interface respresenting a handler of a single client connection.
/// </summary>
public interface IClientHandler
{
    /// <summary>
    /// User name of the handled client. Null if client not logged it.
    /// </summary>
    string HandledUserName { get; set; }

    /// <summary>
    /// True if handler is currently working, false otherwise.
    /// </summary>
    bool Working { get; }
        
    /// <summary>
    /// Starts the work of the handler.
    /// </summary>
    void startWorking();      

    /// <summary>
    /// Sends a message to the handled client.
    /// </summary>
    /// <param name="typeByte">Type of message to send</param>
    /// <param name="message">Message to send</param>
    void sendMessage(byte typeByte, byte[] message);

    /// <summary>
    /// Ends the work of the handler and releases resources
    /// </summary>
    void shutdown();
}

/*
This interface allows for dependency inversion and favours code extension over modification. Single resposibility - as it only has
a few methods essential to handling a client.
*/