using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatModel
{
	public class Conversation
	{
		private string name;
		private int id;
		private List<User> users;
		private Dictionary<int, Message> messages;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int ID
		{
			get
			{
				return id;
			}
		}
		public Conversation(string v1, int v2)
		{
			this.name = v1;
			this.id = v2;
			this.users = new List<User>();
			this.messages = new Dictionary<int, Message>();
		}

		public int getId()
		{
			return id;
		}

		public ICollection<User> getUsers()
		{
			return users;
		}

		public string getName()
		{
			return name;
		}

		public IEnumerable<Message> getMessages()
		{
			return messages.Values;
		}

		public bool matchWithUser(User user1)
		{
			if (users.Contains(user1))
				return false;
			users.Add(user1);
			return true;
		}

		public Message addMessage(User user1, int v1, MessageContent messageContent1, DateTime datetime, int v2)
		{
			if ((messages.ContainsKey(v1) || v1 == -1) && users.Contains(user1) && !messages.ContainsKey(v2))
			{
				Message message = new Message(user1, v1 == -1 ? null : messages[v1], messageContent1, datetime, v2);
				messages.Add(v2, message);
				return message;
			}
			else
			{
				return null;
			}
		}

		public Message addMessage(object v)
		{
			throw new NotImplementedException();
		}

		public Message getMessage(int v)
		{
			if (messages.ContainsKey(v))
				return messages[v];
			return null;
		}

		public bool unmatchWithUser(User user1)
		{
			if (users.Contains(user1))
			{
				users.Remove(user1);
				return true;
			}
			else
			{
				return false;
			}
		}

		public string serialize()
		{
			throw new NotImplementedException();
		}

		public Conversation getUpdates(int v)
		{
			throw new NotImplementedException();
		}
	}
}