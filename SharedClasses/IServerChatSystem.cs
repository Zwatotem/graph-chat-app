using System;
using System.Collections.Generic;

namespace ChatModel
{
    public interface IServerChatSystem : IChatSystem
    {
        UserUpdates getUpdatesToUser(string userName, DateTime t);

        List<Conversation> getConversationsOfUser(string userName);
    }
}
