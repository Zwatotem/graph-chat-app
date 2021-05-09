using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel
{
	public class ClientChatSystem : ChatSystem
	{
		public string getUserName()
		{
			throw new NotImplementedException();
		}

		public void applyUpdates(object p)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<object> getConversationsOfUser(string v)
		{
			throw new NotImplementedException();
		}

		public bool logIn(string v)
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