using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Concrete implementation of IUser.
/// </summary>
[Serializable]
public class User : IUser
{
	private Guid guid;
	private string userName; //unique name of the user
	private List<Guid> conversations; //list of all conversations in which the user participates

	User(User other)
	{
		this.guid = other.guid;
		this.userName = other.userName;
		this.conversations = other.conversations;
	}
	
	public User(string userName, ChatSystem chatSystem)
	{
		this.guid = Guid.NewGuid();
		this.userName = userName;
		this.conversations = new List<Guid>();
		this.ChatSystem = chatSystem;
	}
	
	public User(MemoryStream memoryStream, IDeserializer deserializer)
	{
		User user = deserializer.Deserialize(memoryStream) as User;
		this.guid = user.guid;
		this.userName = user.userName;
		this.conversations = user.conversations;
	}
	public override bool Equals(object? obj)
	{
		if(obj is User other)
		{
			return this.guid == other.guid && this.userName == other.userName;
		}
		return false;
	}

	public Guid ID => guid;
	public string Name { get => userName; }
	[field: NonSerialized]
	public ChatSystem ChatSystem { get; set; }

	public IEnumerable<Conversation> Conversations { get => conversations.Select(x=>ChatSystem.FindConversation(x)); }

	public bool MatchWithConversation(Conversation conversation)
	{
		if (conversation == null || conversations.Contains(conversation.ID)) //cannot match with null conversation or one already matched with user
			return false;
		conversations.Add(conversation.ID);
		return true;
	}

	public bool UnmatchWithConversation(Conversation conversation)
	{
		if (conversation == null || !conversations.Contains(conversation.ID)) //to unmatch, the conversation must exist and be matched with user
		{
			return false;
		}
		else
		{
			conversations.Remove(conversation.ID);
			return true;
		}
	}
	
	public MemoryStream Serialize(ISerializer serializer)
	{
		User copy = new User(this); //we Serialize the copy
		return serializer.Serialize(copy); //to avoid serializing the whole chain of parent messages
	}
}

/*
Compliant with Liskov Substitution - implements properly all interface (contract) methods. Has only one responsibility and only essential logic.
*/