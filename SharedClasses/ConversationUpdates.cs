using System.Collections.Generic;
using System.IO;
using ChatModel.Util;

namespace ChatModel
{
	public class ConversationUpdates : BaseConversation
	{
		public ConversationUpdates(string name, int id) : base(name, id) { }

		public List<Refrence<IUser>> getUsersFull()
		{
			return users;
		}

		public Dictionary<int, Message> getMessagesFull()
		{
			return messages;
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

		public MemoryStream serialize(ISerializer serializer)
		{
			return serializer.serialize(this);
		}
	}
}
