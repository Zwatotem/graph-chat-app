using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatModel
{
	public class Conversation
	{
		private string v1;
		private int v2;

		public Conversation(string v1, int v2)
		{
			this.v1 = v1;
			this.v2 = v2;
		}

		public int getId()
		{
			throw new NotImplementedException();
		}

		public List<User> getUsers()
		{
			throw new NotImplementedException();
		}

		public string getName()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Message> getMessages()
		{
			throw new NotImplementedException();
		}

		public bool matchWithUser(User user1)
		{
			throw new NotImplementedException();
		}

		public Message addMessage(User user1, int v1, MessageContent messageContent1, DateTime datetime)
		{
			throw new NotImplementedException();
		}

		public Message addMessage(User user1, int v1, MessageContent messageContent1, DateTime datetime, int v2)
		{
			throw new NotImplementedException();
		}

		public Message addMessage(object v)
		{
			throw new NotImplementedException();
		}

		public Message getMessage(int v)
		{
			throw new NotImplementedException();
		}

		public bool unmatchWithUser(User user1)
		{
			throw new NotImplementedException();
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