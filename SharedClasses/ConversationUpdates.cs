using System.Collections.Generic;
using System.IO;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Class representing an update to a conversation.
/// </summary>
public class ConversationUpdates : BaseConversation
{
	public ConversationUpdates(string name, int id) : base(name, id) { }

	/// <summary>
	/// Gets the list of "double references to users".
	/// </summary>
	/// <returns>List of Refrence objects referencing users.</returns>
	public List<Refrence<IUser>> getUsersFull()
	{
		return users;
	}

	/// <summary>
	/// Gets the dictionary of messages.
	/// </summary>
	/// <returns>Dictionary of messages indexed by their ids.</returns>
	public Dictionary<int, Message> getMessagesFull()
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
		if (result)
		{
			m.AuthorRef = users.Find(u => u.Reference.Name == m.Author.Name);
		}
		m.setParentUnsafe(messages.GetValueOrDefault(m.TargetId, null));
		return result ? m : null;
	}

	/// <summary>
	/// Fixes the internal structure of messages.
	/// </summary>
	public void converge()
	{
		foreach (Message message in messages.Values) //for all messages sets the parent message
		{
			message.setParentUnsafe(messages.GetValueOrDefault(message.TargetId, null));
		}
	}

	/// <summary>
	/// Serializes the conversation.
	/// </summary>
	/// <param name="serializer">Serializer which is to be used.</param>
	/// <returns>MemoryStream containing serialized conversation.</returns>
	public MemoryStream serialize(ISerializer serializer)
	{
		return serializer.serialize(this);
	}
}

/*
This class has a single responsibility - storing updates to conversation. It complies with Liskov Substitution as it doesn't touch base methods
and delegates for example serialization logic to a specialized interface. By referencing an interface it also realizes dependency inversion.
*/