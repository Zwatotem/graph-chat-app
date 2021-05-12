using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel
{
	public class ClientChatSystem : ChatSystem //concrete class derived from abstract ChatSystem. It represents the chat system on the client side.
	{
		private string userName; //user name of the user that is using the client app and has already logged in

		public ClientChatSystem() : base() //no-arg constructor calling ChatSystem constructor
		{
			this.userName = null; //indicates that there is no logged in user at the start
		}

		public bool logIn(string login) //if there is no logged in user sets userName to parameter (if such user exists in the system)
										//and returns true, else returns false.
		{
			if (userName != null) //if someone is already logged in, returns false
			{
				return false;
			}
			if (users.Find(u => u.Name == login) != null) //if user with user name passed as parameter exists
			{
				userName = login;
				return true;
			}
			else
			{
				return false;
			}
		}

		public string getUserName() //returns the user name of the user currently logged in or null if there is no such user.
		{
			return userName;
		}

		public void applyUpdates(User user1)
        {
			throw new NotImplementedException();
		}

		public Conversation addConversation(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Conversation conv = (Conversation)formatter.Deserialize(stream);
			if (conversations.ContainsKey(conv.ID))
			{
				return null;
			}
			var newUsers = new List<User>();
			foreach (var user in conv.getUsers())
			{
				Predicate<User> p = (u => u.Name == user.Name);
				if (users.Exists(p))
				{
					newUsers.Add(users.Find(p));
				}
				else
				{
					users.Add(user); // Won't use add new user, to save the reference
				}
			}
			foreach(var user in newUsers)
			{
				// Replace 'new' users with ones, that we already have
				Predicate<User> p = (u => u.Name == user.Name);
				conv.unmatchWithUser(conv.getUsers().Find(p));
				conv.matchWithUser(user);
				user.matchWithConversation(conv);
			}
			conversations.Add(conv.ID, conv);
			return conv;
		}
	}
}