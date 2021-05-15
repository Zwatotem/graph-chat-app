using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Util;

namespace ChatModel
{
	[Serializable]
	public class Message
	{
		private Refrence<User> authorRef; // Helper for merging user lists after Conversation deserialization
		private User author;
		private MessageContent content;
		private DateTime sentTime;
		private Message targetedMessage;
		private int targetId; // Redundant value used to recover the message structure after deserialization
		private int id;

		public User Author
		{
			get
			{
				if (authorRef.Reference == null) // Author was probably removed from the conversation
				{
					return author;
				}
				else if (authorRef.Reference != author) // Conversation was merged with a new ChatSystem
				{
					author = authorRef.Reference;
				}
				return author;
			}
		}

		public Refrence<User> AuthorRef
		{
			set
			{
				authorRef = value;
			}
		}

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

		public Message(Refrence<User> user, Message targeted, MessageContent messageContent, DateTime datetime, int id)
		{
			this.authorRef = user;
			this.content = messageContent;
			this.sentTime = datetime;
			this.id = id;
			this.TargetedMessage = targeted;
		}

		/// <summary>
		/// Constructs new message by doing shallow copy of the object provided
		/// </summary>
		/// <param name="other">Template message for construction</param>
		public Message(Message other)
		{
			this.authorRef = other.authorRef;
			this.author = other.author;
			this.content = other.content;
			this.sentTime = other.sentTime;
			this.id = other.id;
			this.targetId = other.targetId;
			this.targetedMessage = other.targetedMessage;
		}

		public Message getParent()
		{
			return targetedMessage;
		}

		public void setParentUnsafe(Message t)
		{
			targetedMessage = t;
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
			return Author;
		}
	}
}