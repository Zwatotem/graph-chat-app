using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChatModel
{
	/// <summary>
	/// Class representing collection of updates to all user's conversations.
	/// </summary>
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

		/// <summary>
		/// Adds conversation updates object to the collection.
		/// </summary>
		/// <param name="conversation">Conversation updates to add.</param>
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

/*
This class's only responsibility is to represent collection of UserUpdates and it only has logic necessary for that purpose.
*/