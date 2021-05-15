using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
	public class UserUpdates : IEnumerable<ConversationUpdates>
	{
		private List<ConversationUpdates> conversations;

		public UserUpdates()
		{
			this.conversations = new List<ConversationUpdates>();
		}
		public UserUpdates(ConversationUpdates[] conversations)
		{
			this.conversations = conversations.ToList<ConversationUpdates>();
		}
		public void addConversation(ConversationUpdates conversation)
		{
			this.conversations.Add(conversation);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)conversations).GetEnumerator();
		}

		public IEnumerator<ConversationUpdates> GetEnumerator()
		{
			return ((IEnumerable<ConversationUpdates>)conversations).GetEnumerator();
		}
	}
}
