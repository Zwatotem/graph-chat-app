using System;

namespace ChatModel
{
	public class Message
	{
		private Conversation containingConversation;
		private User author;
		private int v1;
		private MessageContent content;
		private DateTime sentTime;
		private Message targetedMessage;
		private int id;

		public Conversation ContainingConversation
		{
			get
			{
				return containingConversation;
			}
		}

		public int ID
		{
			get
			{
				return ID;
			}
		}

		public Message(User user1, Message targeted, MessageContent messageContent1, DateTime datetime, int id)
		{
			this.author = user1;
			this.content = messageContent1;
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