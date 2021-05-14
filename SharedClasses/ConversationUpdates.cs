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
				m.AuthorRef = users.Find(u => u.Name == m.Author.Name);
			}
			m.TargetedMessage = messages.GetValueOrDefault(m.TargetId, null);
			return result ? m : null;
		}

		/// <summary>
		/// Fixes the internal structure of messages
		/// </summary>
		public void converge()
		{
			foreach (Message message in messages.Values)
			{
				message.TargetedMessage = messages.GetValueOrDefault(message.TargetId, null);
			}
		}

		public Stream serialize()
		{
			throw new NotImplementedException();
		}
	}
}
