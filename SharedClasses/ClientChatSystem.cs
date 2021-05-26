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

		public void applyUpdates(UserUpdates updates)
		{
			foreach (var convUpdate in updates)
			{
				if (!Conversations.ContainsKey(convUpdate.ID))
				{
					var newUsers = new List<User>(); // List of overlapping User objects
					Conversations.Add(convUpdate.ID, new Conversation(convUpdate));
					foreach (var user in convUpdate.getUsers())
					{
						Predicate<User> p = (u => u.Name == user.Name);
						if (users.Exists(p))
						{
							newUsers.Add(users.Find(p)); // Collect the users we'll inject to the new Conversation
						}
						else
						{
							users.Add(user); // Won't use addNewUser(), to keep conv's reference
						}
					}
					foreach (var user in newUsers)
					{
						// Replace 'new' users with ones, that we already have
						Conversations[convUpdate.ID].reMatchWithUser(user);
						user.matchWithConversation(Conversations[convUpdate.ID]);
					}
					if (freedIds.Contains(convUpdate.ID))
					{
						var stackNoID = new List<int>(freedIds);
						stackNoID.Remove(smallestFreeId);
						freedIds = new Stack<int>(stackNoID);
					}
					else
					{
						for (int i = smallestFreeId; i < convUpdate.ID; i++)
						{
							freedIds.Push(i);
						}
						smallestFreeId = convUpdate.ID + 1;
					}
				}
				else
				{
					Conversations[convUpdate.ID].applyUpdates(convUpdate);
				}
			}
		}

		public Conversation addConversation(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
			Conversation conv = (Conversation)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
			if (Conversations.ContainsKey(conv.ID))
			{
				return null;
			}
			var newUsers = new List<User>(); // List of overlapping User objects
			foreach (var user in conv.getUsers())
			{
				Predicate<User> p = (u => u.Name == user.Name);
				if (users.Exists(p))
				{
					newUsers.Add(users.Find(p)); // Collect the users we'll inject to the new Conversation
				}
				else
				{
					users.Add(user); // Won't use addNewUser(), to keep conv's reference
				}
			}
			foreach (var user in newUsers)
			{
				// Replace 'new' users with ones, that we already have
				conv.reMatchWithUser(user);
				user.matchWithConversation(conv);
			}
			Conversations.Add(conv.ID, conv);
			InvokePropertyChanged(nameof(Conversations));
			return conv;
		}
	}
}