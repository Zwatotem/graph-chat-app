using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatModel.Util;

namespace ChatModel
{
	[Serializable]
	public class Conversation : BaseConversation
	{
		private int smallestFreeId;	

		public Conversation(string name, int id) : base(name, id)
		{
			this.smallestFreeId = 1;
		}

		public Conversation(ConversationUpdates conv) : base()
		{
			this.id = conv.ID;
			this.name = conv.Name;
			this.Users = conv.Users;
			this.messages = conv.getMessagesFull();
			this.smallestFreeId = 1;
			foreach (var k in messages.Keys)
			{
				smallestFreeId = k > smallestFreeId ? k : smallestFreeId;
			}
			smallestFreeId++;
			foreach (var message in messages.Values)
			{
				message.AuthorRef = users.Find(u => u.Reference.Name == message.Author.Name);
			}
			converge();
		}

		public Refrence<IUser> getUserRef(IUser user)
		{
			return users.Find(r => r.Reference == user);
		}

		public bool matchWithUser(IUser user)
		{
			if (Users.Contains(user))
				return false;
			users.Add(new Refrence<IUser>(user));
			return true;
		}

		public bool reMatchWithUser(IUser user)
		{
			var internalUser = Users.Find(u => u.Name == user.Name);
			if (internalUser != null)
			{
				var userRef = getUserRef(internalUser);
				internalUser.unmatchWithConversation(this);
				userRef.Reference = user;
				return true;
			}
			return false;
		}

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

		public Message getMessage(int id)
		{
			if (messages.ContainsKey(id))
				return messages[id];
			return null;
		}

		public Message addMessage(IUser user, int parentID, IMessageContent messageContent1, DateTime datetime)
		{
			int newID = smallestFreeId;
			if ((messages.ContainsKey(parentID) || parentID == -1) && Users.Contains(user) && !messages.ContainsKey(newID))
			{
				Message message = new Message(getUserRef(user), parentID == -1 ? null : messages[parentID], messageContent1, datetime, newID);
				messages.Add(newID, message);
				smallestFreeId++;
				return message;
			}
			else
			{
				return null;
			}
		}

		public Message addMessage(Stream stream, IDeserializer deserializer)
		{
			var formatter = new BinaryFormatter();
			Message mess = (Message)deserializer.deserialize(stream);
			if (mess != null)
			{
				if (mess.ID >= smallestFreeId && (mess.TargetId == -1 || messages.ContainsKey(mess.TargetId)))
				{
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
		/// <returns>Message that was added, or <c>null</c>null in case of error.</returns>
		public Message addMessageUnsafe(Message m)
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
		/// Fixes the internal structure of messages
		/// </summary>
		public void converge()
		{
			foreach (Message message in messages.Values)
			{
				message.Parent = messages.GetValueOrDefault(message.TargetId, null);
			}
		}

		internal void applyUpdates(ConversationUpdates conv)
		{
			users.AddRange(conv.getUsersFull());
			List<int> newMssgIDs = new List<int>();
			foreach (var mess in conv.Messages)
			{
				addMessageUnsafe(mess);
				newMssgIDs.Add(mess.ID);
			}
			foreach (int id in newMssgIDs)
			{
				messages[id].Parent = (messages[id].TargetId == -1) ? null : messages[messages[id].TargetId];
				messages[id].AuthorRef = users.Find(u => u.Reference.Name == messages[id].Author.Name);
			}
		}	

		public ConversationUpdates getUpdates(int lastMessageId)
		{
			var lastMessageTime = getMessage(lastMessageId)?.SentTime;
			return getUpdates(lastMessageTime);
		}

		public ConversationUpdates getUpdates(DateTime? time)
		{
			var updates = new ConversationUpdates(Name, ID);

			updates.Users = Users;

			foreach (var message in messages.Values)
			{
				if (message.SentTime > time)
				{
					updates.addMessageUnsafe(new Message(message));
				}
			}
			updates.converge();
			return updates;
		}

		public MemoryStream serialize(ISerializer serializer)
		{
			return serializer.serialize(this);
		}
	}
}