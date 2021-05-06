using System;

namespace ChatModel
{
	public class Message
	{
		private User author;
		private MessageContent content;
		private DateTime sentTime;
		private Message targetedMessage;
		private int id;

		public int ID
		{
			get
			{
				return ID;
			}
		}

		public Message(User user, Message targeted, MessageContent messageContent, DateTime datetime, int id)
		{
			this.author = user;
			this.content = messageContent;
			this.sentTime = datetime;
			this.id = id;
			this.targetedMessage = targeted;
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

		public object serialize()
		{
			throw new NotImplementedException();
		}

		public User getUser()
		{
			return author;
		}
	}
}