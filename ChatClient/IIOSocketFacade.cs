
namespace ChatClient
{
    /// <summary>
    /// Interface representing basic input-output net socket functionality.
    /// </summary>
    public interface IIOSocketFacade
    {
        /// <summary>
        /// Reads a given number of bytes from the socket.
        /// </summary>
        /// <param name="length">Number of bytes to read</param>
        /// <returns>Byte array of read bytes.</returns>
        byte[] receiveMessage(int length);

        /// <summary>
        /// Sends a message via the socket, preceded by a header with a type byte and message's length.
        /// </summary>
        /// <param name="typeByte">Type byte indicating the kind of content in the message.</param>
        /// <param name="message">Message to send</param>
        void sendMessage(byte typeByte, byte[] message);

        /// <summary>
        /// Closes the socket and releases all resources.
        /// </summary>
        void shutdown();
    }
}

/*
This interface is inspired by the idea of the facade design pattern. It allows for the simplification of socket operations in code.
It realizes dependency inversion, as other code references only this abstract type. It encourages code extension over modification,
in case we want a new implementation. It has only a single responsibility.
*/
