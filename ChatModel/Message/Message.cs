using System;
using System.IO;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Class representing one message in the system
/// </summary>
[Serializable]
public class Message
{
	private Guid authorID; // Double reference is useful for merging user lists after Conversation deserialization
	private IMessageContent content; //content of the message
	private DateTime sentTime;
	private Guid targetId; // Redundant value used to recover the message structure after deserialization
	private Guid id; //unique id of the message
	private Guid conversationID;
	[NonSerialized] private Conversation conversation;

	public Guid ConversationId
	{
		get => conversationID;
		set => conversationID = value;
	} //id of the conversation the message belongs to 


	public Conversation Conversation
	{
		get => conversation;
		set
		{
			conversation = value;
			conversationID = value.ID;
		}
	}

	public Message(IUser user, Guid targeted, IMessageContent messageContent,
		DateTime datetime) //this constructor is mainly for the unit tests
		: this(targeted, messageContent, datetime)
	{
		this.authorID = user.ID;
	}

	public Message(Guid userID, Guid targeted, IMessageContent messageContent, DateTime datetime)
		: this(targeted, messageContent, datetime)
	{
		this.authorID = userID;
	}

	private Message(Guid targeted, IMessageContent messageContent,
		DateTime datetime) //private as it doesn't make sense on its own
	{
		this.content = messageContent;
		this.sentTime = datetime;
		this.id = Guid.NewGuid();
		this.targetId = targeted;
	}

	/// <summary>
	/// Constructs new message by doing shallow copy of the object provided.
	/// </summary>
	/// <param name="other">Template message for construction.</param>
	public Message(Message other)
	{
		this.authorID = other.authorID;
		this.content = other.content;
		this.sentTime = other.sentTime;
		this.id = other.id;
		this.targetId = other.targetId;
		this.conversationID = other.conversationID;
	}

	public Message(Stream stream, IDeserializer deserializer)
	{
		Message mssg = deserializer.Deserialize(stream) as Message;
		this.authorID = mssg.authorID;
		this.content = mssg.content;
		this.sentTime = mssg.sentTime;
		this.id = mssg.id;
		this.targetId = mssg.targetId;
		this.conversationID = mssg.conversationID;
	}

	/// <summary>
	/// Refrence object containing a reference to message's author.
	/// </summary>
	public Guid AuthorID
	{
		get => authorID;
		set => authorID = value;
	}

	/// <summary>
	/// Author of the message.
	/// </summary>
	public IUser Author
	{
		get { return Conversation.ChatSystem.FindUser(this.authorID); }
	}

	/// <summary>
	/// Content of the message.
	/// </summary>
	public IMessageContent Content
	{
		get => content;
	}

	/// <summary>
	/// Time when the message was sent.
	/// </summary>
	public DateTime SentTime
	{
		get => sentTime;
	}

	/// <summary>
	/// Message to which this one is replying.
	/// </summary>
	public Message Parent
	{
		get => Conversation.FindMessage(this.targetId);

		set => TargetId = value?.ID ?? Guid.Empty;
	}

	/// <summary>
	/// Id of the parent message.
	/// </summary>
	public Guid TargetId
	{
		get => targetId;
		private set => targetId = value;
	}

	/// <summary>
	/// Unique id of the message.
	/// </summary>
	public Guid ID
	{
		get => id;
		private set => id = value;
	}

	/// <summary>
	/// Serializes the message.
	/// </summary>
	/// <param name="serializer">ISerializer instance which is to be used</param>
	/// <returns>MemoryStream containing serialized message.</returns>
	public MemoryStream Serialize(ISerializer serializer)
	{
		Message copy = new Message(this); //we Serialize the copy
		return serializer.Serialize(copy); //to avoid serializing the whole chain of parent messages
	}
}

/*
This class is perhaps too big for modern standards, but it's structure came from business analysis department and contact with the client.
Still where it was possible, dependency inversion was implemented (refering to IMessageContent rather than something concrete) 
and the class has more less one responsibility (logic necessary for representing a message, serialization delegated to a specialized interface).  
*/