using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatModel.Util;

namespace ChatModel
{
	/// <summary>
	/// Class representing a conversation.
	/// </summary>
	[Serializable]
	public class Conversation : BaseConversation
	{
		private int smallestFreeId;	//smallest id available to be assigned to a new message (as ids are unique and strictly increasing)

		public Conversation(string name, int id) : base(name, id)
		{
			this.smallestFreeId = 1;
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
			this.smallestFreeId = 1;
			foreach (var k in messages.Keys)
			{
				smallestFreeId = (k > smallestFreeId) ? k : smallestFreeId;
			}
			smallestFreeId++;
			foreach (var message in messages.Values)
			{
				message.AuthorRef = users.Find(u => u.Reference.Name == message.Author.Name); //sets messages' references to their authors
			}
			converge(); //fixes messages' references to their parent messages
		}

		/// <summary>
		/// Fixes the internal structure of messages by setting references to parent messages.
		/// </summary>
		public void converge()
		{
			foreach (Message message in messages.Values)
			{
				message.Parent = messages.GetValueOrDefault(message.TargetId, null); //set reference to the message with the correct id
				//if there is no such message, set it to null
			}
		}

		/// <summary>
		/// Finds and returns Refrence from the list.
		/// </summary>
		/// <param name="user">User to find</param>
		/// <returns>IUser wrapped in Refrence object from users list.</returns>
		public Refrence<IUser> getUserRef(IUser user)
		{
			return users.Find(r => r.Reference == user);
		}

		/// <summary>
		/// Adds user to the list of users currently participating.
		/// </summary>
		/// <param name="user">User to match with the conversation</param>
		/// <returns>True if successful, false otherwise.</returns>
		public bool matchWithUser(IUser user)
		{
			if (Users.Contains(user))
				return false;
			users.Add(new Refrence<IUser>(user));
			return true;
		}

		/// <summary>
		/// Changes the references inside the conversation to an actuall user of the system.
		/// </summary>
		/// <remarks>Used after deserialization.</remarks>
		/// <param name="user">User of the system.</param>
		/// <returns>Returns true if rematched, false otherwise.</returns>
		public bool reMatchWithUser(IUser user)
		{
			var internalUser = Users.Find(u => u.Name == user.Name); //finds the same user as parameter but as different object - relic of serialization
			if (internalUser != null)
			{
				var userRef = getUserRef(internalUser); //gets the Refrence object from the list of users
				internalUser.unmatchWithConversation(this); //unmatches the relic user - it is to be garbage collected
				userRef.Reference = user; //sets the reference to the actuall user of the system
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes a user from the list of currently participating users.
		/// </summary>
		/// <param name="user">User to unmatch from conversation</param>
		/// <returns>True if successful, false otherwise.</returns>
		public bool unmatchWithUser(IUser user)
		{
			if (Users.Contains(user))
			{
				users.RemoveAll(r => r.Reference == user);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a message with the given id.
		/// </summary>
		/// <param name="id">Id of the message to retrieve</param>
		/// <returns>Found message or null if no such message.</returns>
		public Message getMessage(int id)
		{
			if (messages.ContainsKey(id))
				return messages[id];
			return null;
		}

		/// <summary>
		/// Adds a message to conversation.
		/// </summary>
		/// <param name="user">Message's author</param>
		/// <param name="parentID">ID of message to which the new one is replying. -1 indicates not replying to any other message</param>
		/// <param name="messageContent">Content of the message</param>
		/// <param name="datetime">Sent time of the message</param>
		/// <returns>Reference to added message, null if unsuccessful.</returns>
		public Message addMessage(IUser user, int parentID, IMessageContent messageContent, DateTime datetime)
		{
			int newID = smallestFreeId;
			if ((messages.ContainsKey(parentID) || parentID == -1) && Users.Contains(user) && !messages.ContainsKey(newID))
			{
				//can add only if the message isn't replying to anything or targeted message exists and the author exists in the system
				//and there is no other message with this id
				Message message = new Message(getUserRef(user), (parentID == -1) ? null : messages[parentID], messageContent, datetime, newID);
				messages.Add(newID, message);
				smallestFreeId++;
				return message;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Deserializes and adds message from stream.
		/// </summary>
		/// <param name="stream">Stream from which to deserialize</param>
		/// <param name="deserializer">Deserializer which is to be used</param>
		/// <returns>Reference to added message, null if unsuccessful.</returns>
		public Message addMessage(Stream stream, IDeserializer deserializer)
		{
			var formatter = new BinaryFormatter();
			Message mess = (Message)deserializer.deserialize(stream);
			if (mess != null)
			{
				if (mess.ID >= smallestFreeId && (mess.TargetId == -1 || messages.ContainsKey(mess.TargetId)))
				{
					//as in previous method, these conditions had to be checked
					messages.Add(mess.ID, mess);
					mess.Parent = (mess.TargetId == -1) ? null : messages[mess.TargetId];
					smallestFreeId = mess.ID;
				}
				else
                {
					mess = null;
                }
			}
			return mess;
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
			m.Parent = messages.GetValueOrDefault(m.TargetId, null);
			return result ? m : null;
		}

		/// <summary>
		/// Updates the conversation based on received object.
		/// </summary>
		/// <param name="updt">Received update</param>
		public void applyUpdates(ConversationUpdates updt)
		{
			users.AddRange(updt.getUsersFull());
			List<int> newMssgIDs = new List<int>(); //saving ids of all newly added messages
			foreach (var mess in updt.Messages)
			{
				addMessageUnsafe(mess); //first adding messages without properly setting their parent messages and authors
				//as they could possibly be added in an order that would make it impossible
				newMssgIDs.Add(mess.ID);
			}
			foreach (int id in newMssgIDs) //so these references are fixed later on
			{
				messages[id].Parent = (messages[id].TargetId == -1) ? null : messages[messages[id].TargetId]; //setting parent references
				messages[id].AuthorRef = users.Find(u => u.Reference.Name == messages[id].Author.Name); //setting author references
			}
		}

		/// <summary>
		/// Gets updates from a time later than when message with given id was added.
		/// </summary>
		/// <param name="lastMessageId">Last known messsage id</param>
		/// <returns>Updates to conversation.</returns>
		public ConversationUpdates getUpdates(int lastMessageId)
		{
			var lastMessageTime = getMessage(lastMessageId)?.SentTime;
			return getUpdates(lastMessageTime);
		}

		/// <summary>
		/// Gets updates from the time after parameter.
		/// </summary>
		/// <param name="time">Time of last known conversation state</param>
		/// <returns>Updates to conversation.</returns>
		public ConversationUpdates getUpdates(DateTime? time)
		{
			var updates = new ConversationUpdates(Name, ID); //creates new updates object
			updates.Users = Users; //adds to it all users
			foreach (var message in messages.Values)
			{
				if (message.SentTime > time) //and all messages sent after specified time
				{
					updates.addMessageUnsafe(new Message(message)); //must use unsafe version as messages can be added in wrong order
				}
			}
			updates.converge(); //fixes references that might be bad after unsafe adding
			return updates;
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
}

/*
This class is perhaps too big for modern standards, but it's structure came from business analysis department and contact with the client.
Still where it was possible, dependency inversion was implemented (refering to IUser rather than something concrete) and the class has more less
one responsibility (logic necessary for representing a conversation, serialization delegated to a specialized interface). 
It complies with Liskov Substitution as it doesn't meddle with base methods.
*/