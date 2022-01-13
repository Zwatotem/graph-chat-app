using System;
using System.Collections.Generic;
using System.Linq;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Abstract class containing some bare bones conversation fields and methods.
/// </summary>
[Serializable]
public abstract class BaseConversation
{
	protected string name; //name of conversation - doesn't have to be unique
	protected Guid id;

	protected List<Guid> users; //list of users' ids

	protected Dictionary<Guid, Message> messages; //messages indexed by their unique id

	protected BaseConversation()
	{
	}

	public BaseConversation(string name)
	{
		this.name = name;
		this.id = Guid.NewGuid();
		this.users = new List<Guid>();
		this.messages = new Dictionary<Guid, Message>();
	}

	public BaseConversation(string name, Guid id)
	{
		this.name = name;
		this.id = id;
		this.users = new List<Guid>();
		this.messages = new Dictionary<Guid, Message>();
	}
	
	[field: NonSerialized]
	public ChatSystem ChatSystem { get; set; }

	/// <summary>
	/// Conversation's name (not unique).
	/// </summary>
	public string Name => name;

	/// <summary>
	/// Conversation's ID (unique).
	/// </summary>
	public Guid ID => id;

	/// <summary>
	/// List of all participating users.
	/// </summary>
	public virtual IEnumerable<IUser> Users
	{
		get { return users.Select(id => ChatSystem.FindUser(id)); }
		set { users = value.Select(u => u.ID).ToList(); }
	}

	/// <summary>
	/// Collection of all messages in the converastion.
	/// </summary>
	public IEnumerable<Message> Messages
	{
		get => messages.Values;
	}
}

/*
The only purpose of the class is to allow declaration and access to some basic properties of a conversation.
Dependency inversion - not referencing user class, but rather abstract IUser.
In our implementation this class contains common properties of Conversation and ConversationUpdates classes.
*/