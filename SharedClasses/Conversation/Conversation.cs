using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Class representing a conversation.
/// </summary>
[Serializable]
public class Conversation : BaseConversation, INotifyPropertyChanged
{
	[field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
	private void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged(sender, args);

	public Conversation(string name) : base(name)
	{
	}

	public Conversation(string name, Guid id) : base(name, id)
	{
	}


	public ObservableCollection<IUser> ObservableUsers
	{
		get
		{
			var oc = new ObservableCollection<IUser>();
			foreach (IUser user in Users)
			{
				oc.Add(user);
			}

			return oc;
		}
	}

	public ObservableCollection<Message> ObservableMessages
	{
		get
		{
			var oc = new ObservableCollection<Message>();
			foreach (var message in messages)
			{
				oc.Add(message.Value);
			}

			return oc;
		}
	}

	/// <summary>
	/// Creates a new conversation from a conversation update.
	/// </summary>
	/// <remarks>Currently unused.</remarks>
	/// <param name="conv">Conversation update</param>
	public Conversation(ConversationUpdates conv) : base()
	{
		this.id = conv.ID;
		this.name = conv.Name;
		this.Users = conv.Users;
		this.messages = conv.getMessagesFull();
	}

	/// <summary>
	/// Finds and returns Refrence from the list.
	/// </summary>
	/// <param name="user">User to find</param>
	/// <returns>IUser wrapped in Refrence object from users list.</returns>
	public Guid GetUserID(IUser user)
	{
		return users.FirstOrDefault(id => id == user.ID, Guid.Empty);
	}

	/// <summary>
	/// Adds user to the list of users currently participating.
	/// </summary>
	/// <param name="user">User to match with the conversation</param>
	/// <returns>True if successful, false otherwise.</returns>
	public bool MatchWithUser(IUser user)
	{
		if (Users.Contains(user))
			return false;
		users.Add(user.ID);
		return true;
	}

	/// <summary>
	/// Removes a user from the list of currently participating users.
	/// </summary>
	/// <param name="user">User to unmatch from conversation</param>
	/// <returns>True if successful, false otherwise.</returns>
	public bool UnMatchWithUser(IUser user)
	{
		if (Users.Contains(user))
		{
			users.RemoveAll(id => id == user.ID);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Adds a message to conversation.
	/// </summary>
	/// <param name="user">Message's author</param>
	/// <param name="parentID">id of message to which the new one is replying. -1 indicates not replying to any other message</param>
	/// <param name="messageContent">Content of the message</param>
	/// <param name="datetime">Sent time of the message</param>
	/// <returns>Reference to added message, null if unsuccessful.</returns>
	public Message AddMessage(IUser user, Guid parentID, IMessageContent messageContent, DateTime datetime)
	{
		//can add only if the message isn't replying to anything or targeted message exists and the author exists in the system
		//and there is no other message with this id

		var message = new Message(user.ID, parentID, messageContent, datetime) { Conversation = this };
		messages.Add(message.ID, message);
		return message;
	}


	public Message FindMessage(Guid id)
	{
		return id != Guid.Empty ? messages[id] : null;
	}


	/// <summary>
	/// Deserializes and adds message from stream.
	/// </summary>
	/// <param name="stream">Stream from which to Deserialize</param>
	/// <param name="deserializer">Deserializer which is to be used</param>
	/// <returns>Reference to added message, null if unsuccessful.</returns>
	public Message AddMessage(Stream stream, IDeserializer deserializer)
	{
		var mess = deserializer.Deserialize(stream) as Message;
		if (mess == null) return null;
		if (!messages.ContainsKey(mess.ID) && (messages.ContainsKey(mess.TargetId) || mess.TargetId == Guid.Empty))
		{
			//as in previous method, these conditions had to be checked
			messages.Add(mess.ID, mess);
			mess.Conversation = this;
			OnPropertyChanged(this, new(nameof(ObservableMessages)));
		}
		else
		{
			mess = null;
		}

		return mess;
	}

	/// <summary>
	/// Adds a specified message to the conversation.
	/// </summary>
	/// <remarks>
	/// <strong>The specified message will be added despite not matching a valid parent.</strong>
	/// </remarks>
	/// <param name="mess">Message object to add</param>
	/// <returns>Message that was added, or <c>null</c> in case of error.</returns>
	internal Message addMessageUnsafe(Message mess)
	{
		var result = messages.TryAdd(mess.ID, mess);
		if (result)
		{
			OnPropertyChanged(this, new(nameof(ObservableMessages)));
		}

		mess.Parent = messages.GetValueOrDefault(mess.TargetId, null);
		return result ? mess : null;
	}

	/// <summary>
	/// Updates the conversation based on received object.
	/// </summary>
	/// <param name="updt">Received update</param>
	public void ApplyUpdates(ConversationUpdates updt)
	{
		this.ChatSystem.Users = ChatSystem.Users.Union(updt.getUsersFull());
		users.AddRange(updt.getUsersFull().Select(u => u.ID));
		foreach (var user in updt.Users)
		{
			user.ChatSystem = ChatSystem;
		}

		foreach (var mess in updt.Messages)
		{
			addMessageUnsafe(mess);
			mess.Conversation = this;
		}
	}

	/// <summary>
	/// Gets updates from a time later than when message with given id was added.
	/// </summary>
	/// <param name="lastMessageId">Last known messsage id</param>
	/// <returns>Updates to conversation.</returns>
	public ConversationUpdates GetUpdates(Guid lastMessageId)
	{
		var lastMessageTime = FindMessage(lastMessageId)?.SentTime;
		return GetUpdates(lastMessageTime);
	}

	/// <summary>
	/// Gets updates from the time after parameter.
	/// </summary>
	/// <param name="time">Time of last known conversation state</param>
	/// <returns>Updates to conversation.</returns>
	public ConversationUpdates GetUpdates(DateTime? time)
	{
		var updates = new ConversationUpdates(Name, ID); //creates new updates object
		updates.Users = Users; //adds to it all users
		foreach (var message in messages.Values)
		{
			if (message.SentTime > time) //and all messages sent after specified time
			{
				//must use unsafe version as messages can be added in wrong order
				updates.addMessageUnsafe(new Message(message));
			}
		}

		return updates;
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
This class is perhaps too big for modern standards, but it's structure came from business analysis department and contact with the client.
Still where it was possible, dependency inversion was implemented (refering to IUser rather than something concrete) and the class has more less
one responsibility (logic necessary for representing a conversation, serialization delegated to a specialized interface). 
It complies with Liskov Substitution as it doesn't meddle with base methods.
*/