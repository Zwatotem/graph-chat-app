using ChatModel.Util;
using System;
using System.IO;

namespace ChatModel
{
	/// <summary>
	/// Interface representing general chat system functionality.
	/// </summary>
	/// <remarks>Used when it's unimportant to consider specific client or server side functionality.</remarks>
    public interface IChatSystem
    {
		/// <summary>
		/// Gets a user with a specified user name.
		/// </summary>
		/// <param name="userName">User name to find</param>
		/// <returns>Reference to found user or null if there isn't a user with given name.</returns>
		IUser getUser(string userName);

		/// <summary>
		/// Adds new user to the system if proposed name available.
		/// </summary>
		/// <param name="newUserName">User name of the new user</param>
		/// <returns>Reference to created user or null if user name already taken.</returns>
		IUser addNewUser(string newUserName);

		/// <summary>
		/// Gets a conversation with a specified id.
		/// </summary>
		/// <param name="id">Conversation id to find</param>
		/// <returns>Reference to found conversation or null if there isn't a conversation with given id.</returns>
		Conversation getConversation(int id);

		/// <summary>
		/// Creates new conversation.
		/// </summary>
		/// <param name="conversationName">Name of the conversation to create</param>
		/// <param name="ownersNames">Array of user names of users which are to participate</param>
		/// <returns>Reference to created conversation or null if some of the users don't exist.</returns>
		Conversation addConversation(string conversationName, params string[] ownersNames);

		/// <summary>
		/// Creates new conversation.
		/// </summary>
		/// <param name="conversationName">Name of the conversation to create</param>
		/// <param name="owners">Array of references to users which are to participate</param>
		/// <returns>Returns reference to created conversation, or null if unsuccessful.</returns>
		Conversation addConversation(string conversationName, params IUser[] owners);

		/// <summary>
		/// Adds a user to a conversation.
		/// </summary>
		/// <param name="userName">Name of user to add</param>
		/// <param name="id">Id of conversation to which to add</param>
		/// <returns>True if successful, false otherwise.</returns>
		bool addUserToConversation(string userName, int id);

		/// <summary>
		/// Removes a user from a conversation.
		/// </summary>
		/// <param name="userName">User name of user to remove</param>
		/// <param name="id">Id of conversation from which to remove</param>
		/// <returns>True if successful, false otherwise.</returns>
		bool leaveConversation(string userName, int id);

		/// <summary>
		/// Sends a message in a conversation.
		/// </summary>
		/// <param name="convId">Id of conversation</param>
		/// <param name="userName">User name of message's author</param>
		/// <param name="targetId">Id of a message to which the new one is replying</param>
		/// <param name="messageContent">Content of the message to send</param>
		/// <param name="sentTime">Sent time of the message</param>
		/// <returns>Reference to created message if successful, null otherwise</returns>
		Message sendMessage(int convId, string userName, int targetId, IMessageContent messageContent, DateTime sentTime);
	}
}

/*
This interface has a few methods but all of them realise essentially the same responsibility - representing functionality of a chat system.
Interface structure enables dependency inversion (other classes reference this interface instead of a concrete class. This interface also
favours code extension over modification (rather than changing concrete implementations one can just implement the interface anew).
For moder standards there may be too many methods in this interface, but functionalities of chat system very given directly from business
analysis department.
*/