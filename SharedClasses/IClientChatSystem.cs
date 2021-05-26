using System.IO;
using ChatModel.Util;

namespace ChatModel
{
    /// <summary>
    /// Interface extending IChatSystem and adding functionality necessary on the client side of the app.
    /// </summary>
    public interface IClientChatSystem : IChatSystem
    {
        /// <summary>
        /// Name of the currently logged in user.
        /// </summary>
        /// <remarks>Null if no user logged in.</remarks>
        string LoggedInName { get; }

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <param name="login">User name under which to log in.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool logIn(string login);

        /// <summary>
        /// Deserializes and adds a conversation to the chat system from stream.
        /// </summary>
        /// <param name="stream">Serialized conversation in stream form</param>
        /// <param name="deserializer">Deserializer to be used</param>
        /// <returns></returns>
        Conversation addConversation(Stream stream, IDeserializer deserializer);

        /// <summary>
        /// Updates the state of chat system based on UserUpdates object.
        /// </summary>
        /// <param name="updates">Object containig updates to apply</param>
        void applyUpdates(UserUpdates updates);
    }
}

/*
This interface has only a few essential methods, doesn't meddle with base inherited ones (thus complying with Liskov Substitution),
allows other classes to referene it rather than something concrete (dependency inversion) and favours code extension over modification
- when one wants the client chat system to behave differently one can just implement the interface anew instead of modifying existing code.
*/