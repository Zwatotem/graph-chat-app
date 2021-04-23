using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatModel
{
	public class User
	{
		private string v;

		public User(string v)
		{
			this.v = v;
		}

		public ICollection<Conversation> getConversations()
		{
			throw new NotImplementedException();
		}

		public String getName()
		{
			throw new NotImplementedException();
		}

		public bool matchWithConversation(Conversation conversation1)
		{
			throw new NotImplementedException();
		}

		public bool unmatchWithConversation(Conversation conversation1)
		{
			throw new NotImplementedException();
		}
	}
}
