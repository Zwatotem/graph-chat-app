using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatModel
{
	public class ServerChatSystem : ChatSystem //concrete class derived from abstract ChatSystem. It represents the chat system on the server side.
	{

		public ServerChatSystem() : base() { } //no-arg constructor calling ChatSystem constructor

		public User getUpdatesToUser(string v, DateTime t)
		{
			throw new NotImplementedException();
		}

		public List<Conversation> getConversationsOfUser(string userName) //method to return a list of all conversations of user with user name
		//passed as parameter. If successful returns the collection, else (eg. if there is no such user) returns null.
		{
			User user = getUser(userName); //gets reference to the user whose conversations are to be returned
			if (user == null)
            {
				return null; //if there is no such user, return null
            }
			return user.getConversations(); //returns list the of conversations of specified user.
		}
	}
}