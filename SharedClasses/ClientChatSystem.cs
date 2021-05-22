using System;
using System.Collections.Generic;
using System.IO;
using ChatModel.Util;

namespace ChatModel
{
	public class ClientChatSystem : ChatSystem, IClientChatSystem //concrete class derived from abstract IChatSystem. It represents the chat system on the client side.
	{
		private string userName; //user name of the user that is using the client app and has already logged in

		public ClientChatSystem() : base() //no-arg constructor calling IChatSystem constructor
		{
			this.userName = null; //indicates that there is no logged in user at the start
		}

		public string LoggedInName { get => userName; }

		public bool logIn(string login) //if there is no logged in user sets userName to parameter (if such user exists in the system)
										//and returns true, else returns false.
		{
			if (userName == null && users.Find(u => u.Name == login) != null) //if no one is logged in and the user exists
			{
				userName = login; //it is possible to login
				return true;
			}
			else
			{
				return false;
			}
		}

		public Conversation addConversation(Stream stream, IDeserializer deserializer)
		{
			Conversation conv = (Conversation)deserializer.deserialize(stream);
			if (conversations.ContainsKey(conv.ID))
			{
				return null;
			}
			var newUsers = new List<IUser>(); // List of overlapping IUser objects
			foreach (var user in conv.Users)
			{
				Predicate<IUser> p = (u => u.Name == user.Name);
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
			conversations.Add(conv.ID, conv);
			return conv;
		}

		public void applyUpdates(UserUpdates updates)
		{
			foreach (var convUpdate in updates)
			{
				if (!conversations.ContainsKey(convUpdate.ID))
				{
					var newUsers = new List<IUser>(); // List of overlapping IUser objects
					conversations.Add(convUpdate.ID, new Conversation(convUpdate));
					foreach (var user in convUpdate.Users)
					{
						Predicate<IUser> p = (u => u.Name == user.Name);
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
						conversations[convUpdate.ID].reMatchWithUser(user);
						user.matchWithConversation(conversations[convUpdate.ID]);
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
					conversations[convUpdate.ID].applyUpdates(convUpdate);
				}
			}
		}
	}
}