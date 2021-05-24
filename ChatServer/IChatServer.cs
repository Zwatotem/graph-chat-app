
namespace ChatServer
{
    /// <summary>
    /// Interface representing the server side of the chat system and conducting interactions with clients.
    /// </summary>
    public interface IChatServer
    {
        /// <summary>
        /// True is the server is currently working, false otherwise.
        /// </summary>
        bool Working { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        void startServer();

        /// <summary>
        /// Shuts the server down.
        /// </summary>
        void shutdown();
    }
}

/*
This interface represents the abstract concept of the chat server. It has a single purpose, enables dependency inversion,
encourages extension over modification.
*/
