using System;
using System.Collections.Generic;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Abstract class containing some bare bones conversation fields and methods.
/// </summary>
[Serializable]
public abstract class BaseConversation
{
	protected string name; //name of conversation - doesn't have to be unique
	protected int id; //unique id
	protected List<Refrence<IUser>> users; //list of "double references" to users participating (single reference is problematic to serialize)
	protected Dictionary<int, Message> messages; //messages indexed by their unique id

	protected BaseConversation() { }

	public BaseConversation(string name, int id)
	{
		this.name = name;
		this.id = id;
		this.users = new List<Refrence<IUser>>();
		this.messages = new Dictionary<int, Message>();
	}

	/// <summary>
	/// Conversation's name (not unique).
	/// </summary>
	public string Name { get => name; }

	/// <summary>
	/// Conversation's ID (unique).
	/// </summary>
	public int ID { get => id; }

	/// <summary>
	/// List of all participating users.
	/// </summary>
	public List<IUser> Users
	{
		get
		{
			var list = new List<IUser>();
			foreach (var user in users)
			{
				list.Add(user.Reference);
			}
			return list;
		}
		set
		{
			users = new List<Refrence<IUser>>();
			foreach (var user in value)
			{
				users.Add(new Refrence<IUser>(user));
			}
		}
	}

	/// <summary>
	/// Collection of all messages in the converastion.
	/// </summary>
	public ICollection<Message> Messages { get => messages.Values; }
}

/*
The only purpose of the class is to allow declaration and access to some basic properties of a conversation.
Dependency inversion - not referencing user class, but rather abstract IUser.
In our implementation this class contains common properties of Conversation and ConversationUpdates classes.
*/