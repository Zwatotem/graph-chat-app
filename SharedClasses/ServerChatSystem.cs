using System;
using System.Collections.Generic;

namespace ChatModel
{
	public class ServerChatSystem : ChatSystem, IServerChatSystem //concrete class derived from abstract IChatSystem. It represents the chat system on the server side.
	{

		public ServerChatSystem() : base() { } //no-arg constructor calling IChatSystem constructor

		public UserUpdates getUpdatesToUser(string userName, DateTime t)
		{
			var user = users.Find(u => u.Name == userName);
			var updates = new UserUpdates();
			foreach (var conv in user.Conversations)
			{
				updates.addConversation(conv.getUpdates(t));
			}
			return updates;
		}

		public List<Conversation> getConversationsOfUser(string userName) //method to return a list of all conversations of user with user name
																		  //passed as parameter. If successful returns the collection, else (eg. if there is no such user) returns null.
		{
			IUser user = getUser(userName); //gets reference to the user whose conversations are to be returned
			if (user == null)
			{
				return null; //if there is no such user, return null
			}
			return user.Conversations; //returns list the of conversations of specified user.
		}
	}
}