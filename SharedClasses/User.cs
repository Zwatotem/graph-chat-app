using System;
using System.Collections.Generic;

namespace ChatModel;

/// <summary>
/// Concrete implementation of IUser.
/// </summary>
[Serializable]
public class User : IUser
{
	private string userName; //unique name of the user
	private List<Conversation> conversations; //list of all conversations in which the user participates

	public User(string userName)
	{
		this.userName = userName;
		this.conversations = new List<Conversation>();
	}

	public string Name { get => userName; }

	public List<Conversation> Conversations { get => conversations; }

	public bool matchWithConversation(Conversation conversation)
	{
		if (conversation == null || conversations.Exists(c => c.ID == conversation.ID)) //cannot match with null conversation or one already matched with user
			return false;
		conversations.Add(conversation);
		return true;
	}

	public bool unmatchWithConversation(Conversation conversation)
	{
		if (conversation == null || !conversations.Contains(conversation)) //to unmatch, the conversation must exist and be matched with user
		{
			return false;
		}
		else
		{
			conversations.Remove(conversation);
			return true;
		}
	}
}

/*
Compliant with Liskov Substitution - implements properly all interface (contract) methods. Has only one responsibility and only essential logic.
*/