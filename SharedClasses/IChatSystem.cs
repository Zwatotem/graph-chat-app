using System;

namespace ChatModel
{
    public interface IChatSystem
    {
		IUser getUser(string userName);

		IUser addNewUser(string newUserName);

		Conversation getConversation(int id);

		Conversation addConversation(string conversationName, params string[] ownersNames);

		Conversation addConversation(string conversationName, params IUser[] owners);

		bool addUserToConversation(string userName, int id);

		bool leaveConversation(string userName, int id);

		Message sendMessage(int id, string userName, int messageId, IMessageContent messageContent, DateTime sentTime);
	}
}
