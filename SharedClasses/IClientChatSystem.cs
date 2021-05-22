using System.IO;
using ChatModel.Util;

namespace ChatModel
{
    public interface IClientChatSystem : IChatSystem
    {
        string LoggedInName { get; }

        bool logIn(string login);

        Conversation addConversation(Stream stream, IDeserializer deserializer);

        void applyUpdates(UserUpdates updates);
    }
}
