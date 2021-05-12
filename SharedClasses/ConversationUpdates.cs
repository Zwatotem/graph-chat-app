using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
	public class ConversationUpdates : IConversation
	{
		private string name;
		private int id;
		private List<User> users;
		private Dictionary<int, Message> messages;

		public string Name
		{
			get => name;
			set => name = value;
		}
		public int ID => id;
		public List<User> Users
		{
			get => users;
			set => users = value;
		}

		public ConversationUpdates(string name, int id)
		{
			this.name = name;
			this.id = id;
			this.users = new List<User>();
			this.messages = new Dictionary<int, Message>();
		}

		public int getId()
		{
			return id;
		}

		public List<User> getUsers()
		{
			return users;
		}

		public string getName()
		{
			return name;
		}

		public ICollection<Message> getMessages()
		{
			return messages.Values;
		}

		public Message getMessage(int id)
		{
			if (messages.ContainsKey(id))
				return messages[id];
			return null;
		}

		public Message addMessage(Message m)
		{
			return messages.TryAdd(m.ID, m) ? m : null;
		}

		public Stream serialize()
		{
			throw new NotImplementedException();
		}
	}
}
