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
    private Refrence<IUser> authorRef; // Double reference is useful for merging user lists after Conversation deserialization
    private IUser author; // (slightly redundant) reference to message's author
    private IMessageContent content; //content of the message
    private DateTime sentTime;
    private Message targetedMessage; //message to which this one is replying
    private int targetId; // Redundant value used to recover the message structure after deserialization
    private int id; //unique id of the message

    public Message(IUser user, Message targeted, IMessageContent messageContent, DateTime datetime, int id) //this constructor is mainly for the unit tests
        : this(targeted, messageContent, datetime, id) 
    {
        this.author = user;
    }

    public Message(Refrence<IUser> user, Message targeted, IMessageContent messageContent, DateTime datetime, int id)
        : this(targeted, messageContent, datetime, id)
    {
        this.authorRef = user;
    }

    private Message(Message targeted, IMessageContent messageContent, DateTime datetime, int id) //private as it doesn't make sense on its own
    {
        this.content = messageContent;
        this.sentTime = datetime;
        this.id = id;
        this.Parent = targeted;
    }

    /// <summary>
    /// Constructs new message by doing shallow copy of the object provided.
    /// </summary>
    /// <param name="other">Template message for construction.</param>
    public Message(Message other)
    {
        this.authorRef = other.authorRef;
        this.author = other.author;
        this.content = other.content;
        this.sentTime = other.sentTime;
        this.id = other.id;
        this.targetId = other.targetId;
        this.targetedMessage = other.targetedMessage;
    }

    /// <summary>
    /// Refrence object containing a reference to message's author.
    /// </summary>
    public Refrence<IUser> AuthorRef { get => authorRef; set => authorRef = value; }

    /// <summary>
    /// Author of the message.
    /// </summary>
    public IUser Author
    {
        get
        {
            //if authorRef.Reference == null then author was probably removed from the conversation
            if (authorRef.Reference != null && authorRef.Reference != author) // if authorRef is set and doesn't match with author
            {
                author = authorRef.Reference; //set author properly before returning
            }
            return author;
        }
    }

    /// <summary>
    /// Content of the message.
    /// </summary>
    public IMessageContent Content { get => content; }

    /// <summary>
    /// Time when the message was sent.
    /// </summary>
    public DateTime SentTime { get => sentTime; }

    /// <summary>
    /// Message to which this one is replying.
    /// </summary>
    public Message Parent
    {
        get => targetedMessage;
        set
        {
            targetedMessage = value;
            targetId = (value == null) ? -1 : value.ID;
        }
    }

    /// <summary>
    /// Id of the parent message.
    /// </summary>
    public int TargetId { get => targetId; }

    /// <summary>
    /// Unique id of the message.
    /// </summary>
    public int ID { get => id; }       

    /// <summary>
    /// Sets parent message without any validation.
    /// </summary>
    /// <param name="t">Message to set parent to.</param>
    internal void setParentUnsafe(Message t)
    {
        targetedMessage = t;
    }

    /// <summary>
    /// Serializes the message.
    /// </summary>
    /// <param name="serializer">ISerializer instance which is to be used</param>
    /// <returns>MemoryStream containing serialized message.</returns>
    public MemoryStream serialize(ISerializer serializer)
    {
        Message copy = new Message(this); //we serialize the copy
        copy.targetedMessage = null; //after setting its parent to null
        return serializer.serialize(copy); //to avoid serializing the whole chain of parent messages
    }
}

/*
This class is perhaps too big for modern standards, but it's structure came from business analysis department and contact with the client.
Still where it was possible, dependency inversion was implemented (refering to IMessageContent rather than something concrete) 
and the class has more less one responsibility (logic necessary for representing a message, serialization delegated to a specialized interface).  
*/