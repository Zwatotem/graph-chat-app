using System;
using System.Collections.Generic;

namespace ChatModel
{
	[Serializable]
	public class User : IUser //class representing a user of the system
	{
		private string userName; //unique name of the user
		private List<Conversation> conversations; //list of all conversations in which the user participates

		public User(string userName) //constructor setting the name of the user
		{
			this.userName = userName;
			this.conversations = new List<Conversation>();
		}

		public string Name { get => userName; }

		public List<Conversation> Conversations { get => conversations; }

		public bool matchWithConversation(Conversation conversation1) //method to assign a specific conversation to user
																	  //return true is assignment successful. If the conversation1 is already assigned to user (or is null), returns false.
		{
			if (conversation1 == null || conversations.Contains(conversation1))
				return false;
			conversations.Add(conversation1);
			return true;
		}

		public bool unmatchWithConversation(Conversation conversation1) //analogus method to remove a conversation from the list of conversations
																		//assigned to a user. Returns true is operation successful. Returns false if conversation1 is null or isn't matched with user 
																		//when calling the method.
		{
			if (conversation1 == null || !conversations.Contains(conversation1))
			{
				return false;
			}
			else
			{
				conversations.Remove(conversation1);
				return true;
			}
		}
	}
}
