using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ChatModel
{
	public class ConversationUpdates : IConversation
	{
		private string name;
		private int id;
		private List<Refrence<User>> users;
		private Dictionary<int, Message> messages;
		private int smallestFreeId;

		public List<User> Users
		{
			get
			{
				var list = new List<User>();
				foreach (var user in users)
				{
					list.Add(user);
				}
				return list;
			}
			set
			{
				users = new List<Refrence<User>>();
				foreach (var user in value)
				{
					users.Add(user);
				}
			}
		}
		public string Name
		{
			get => name;
			set => name = value;
		}
		public int ID => id;

		public ConversationUpdates(string name, int id)
		{
			this.name = name;
			this.id = id;
			this.users = new List<Refrence<User>>();
			this.messages = new Dictionary<int, Message>();
		}

		public int getId()
		{
			return id;
		}

		public List<User> getUsers()
		{
			return Users;
		}

		public List<Refrence<User>> getUsersFull()
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

		public Dictionary<int, Message> getMessagesFull()
		{
			return messages;
		}

		public Message getMessage(int id)
		{
			if (messages.ContainsKey(id))
				return messages[id];
			return null;
		}

		/// <summary>
		/// Adds a specified message to the conversation.
		/// </summary>
		/// <remarks>
		/// <strong>The specified message will be added despite not matching a valid parent.</strong>
		/// </remarks>
		/// <param name="m">Message object to add</param>
		/// <returns>Message that was added, or <c>null</c>null in case of error.</returns>
		public Message addMessageUnsafe(Message m)
		{
			var result = messages.TryAdd(m.ID, m);
			if (result)
			{
				m.AuthorRef = users.Find(u => u.Reference.Name == m.Author.Name);
			}
			m.setParentUnsafe(messages.GetValueOrDefault(m.TargetId, null));
			return result ? m : null;
		}

		/// <summary>
		/// Fixes the internal structure of messages
		/// </summary>
		public void converge()
		{
			foreach (Message message in messages.Values)
			{
				message.setParentUnsafe(messages.GetValueOrDefault(message.TargetId, null));
			}
		}

		public MemoryStream serialize()
		{
			MemoryStream stream = new MemoryStream();
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, this);
			stream.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
