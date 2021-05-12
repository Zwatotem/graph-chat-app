using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel
{
	[Serializable]
	public class Message
	{
		private User author;
		private MessageContent content;
		private DateTime sentTime;
		private Message targetedMessage;
		private int targetId; // Redundant value used to recover the message structure after deserialization
		private int id;

		public Message TargetedMessage
		{
			get => targetedMessage;
			set
			{
				targetedMessage = value;
				targetId = value == null ? -1 : value.ID;
			}
		}

		public int ID { get => id; }
		public int TargetId { get => targetId; }

		public Message(User user, Message targeted, MessageContent messageContent, DateTime datetime, int id)
		{
			this.author = user;
			this.content = messageContent;
			this.sentTime = datetime;
			this.id = id;
			this.TargetedMessage = targeted;
		}

		public Message getParent()
		{
			return targetedMessage;
		}

		public int getId()
		{
			return id;
		}

		public DateTime getTime()
		{
			return sentTime;
		}

		public MessageContent getContent()
		{
			return content;
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

		public User getUser()
		{
			return author;
		}
	}
}