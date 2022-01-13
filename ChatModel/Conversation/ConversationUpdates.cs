using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Class representing an update to a conversation.
/// </summary>
[Serializable]
public class ConversationUpdates : BaseConversation
{
	private List<IUser> users;

	public override IEnumerable<IUser> Users
	{
		get => users;
		set => users = value.ToList();
	}
	public ConversationUpdates(string name, Guid id) : base(name, id) { }
	
	public ConversationUpdates(MemoryStream memStream, IDeserializer deserializer)
	{
		var @new = deserializer.Deserialize(memStream) as ConversationUpdates;
		this.messages = @new.messages;
		this.name = @new.name;
		this.id = @new.id;
		this.users = @new.users;
	}

	/// <summary>
	/// Gets the list of "double references to users".
	/// </summary>
	/// <returns>List of Refrence objects referencing users.</returns>
	public IEnumerable<IUser> getUsersFull()
	{
		return users;
	}

	/// <summary>
	/// Gets the dictionary of messages.
	/// </summary>
	/// <returns>Dictionary of messages indexed by their ids.</returns>
	public Dictionary<Guid, Message> getMessagesFull()
	{
		return messages;
	}

	/// <summary>
	/// Adds a specified message to the conversation.
	/// </summary>
	/// <remarks>
	/// <strong>The specified message will be added despite not matching a valid parent.</strong>
	/// </remarks>
	/// <param name="m">Message object to add</param>
	/// <returns>Message that was added, or <c>null</c> in case of error.</returns>
	internal Message addMessageUnsafe(Message m)
	{
		var result = messages.TryAdd(m.ID, m);
		return result ? m : null;
	}

	/// <summary>
	/// Serializes the conversation.
	/// </summary>
	/// <param name="serializer">Serializer which is to be used.</param>
	/// <returns>MemoryStream containing serialized conversation.</returns>
	public MemoryStream Serialize(ISerializer serializer)
	{
		return serializer.Serialize(this);
	}
}

/*
This class has a single responsibility - storing updates to conversation. It complies with Liskov Substitution as it doesn't touch base methods
and delegates for example serialization logic to a specialized interface. By referencing an interface it also realizes dependency inversion.
*/