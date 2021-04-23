using System;

namespace ChatModel
{
	public class Message
	{
		private User user1;
		private int v1;
		private MessageContent messageContent1;
		private DateTime datetime;
		private int v2;

		public Message(User user1, int v1, MessageContent messageContent1, DateTime datetime, int v2)
		{
			this.user1 = user1;
			this.v1 = v1;
			this.messageContent1 = messageContent1;
			this.datetime = datetime;
			this.v2 = v2;
		}

		public Message getParent()
		{
			throw new NotImplementedException();
		}

		public int getId()
		{
			throw new NotImplementedException();
		}

		public DateTime getTime()
		{
			throw new NotImplementedException();
		}

		public MessageContent getContent()
		{
			throw new NotImplementedException();
		}

		public object serialize()
		{
			throw new NotImplementedException();
		}

		public User getUser()
		{
			throw new NotImplementedException();
		}
	}
}