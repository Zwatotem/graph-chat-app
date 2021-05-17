using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Util;

namespace ChatModel
{
	[Serializable]
	public class Conversation : IConversation
	{
		private string name;
		private int id;
		private List<Refrence<User>> users;
		private Dictionary<int, Message> messages;
		private int smallestFreeId;

		public List<User> Users
		{
			get
			{
				var list = new List<User>();
				foreach (var user in users)
				{
					list.Add(user);
				}
				return list;
			}
			set
			{
				users = new List<Refrence<User>>();
				foreach (var user in value)
				{
					users.Add(user);
				}
			}
		}

		public string Name
		{
			get => name;
			set => name = value;
		}
		public int ID => id;

		public Conversation(string name, int id)
		{
			this.name = name;
			this.id = id;
			this.users = new List<Refrence<User>>();
			this.messages = new Dictionary<int, Message>();
			this.smallestFreeId = 1;
		}

		public Conversation(ConversationUpdates conv)
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

		public int getId()
		{
			return id;
		}

		public List<User> getUsers()
		{
			return Users;
		}

		public Refrence<User> getUserRef(User user)
		{
			return users.Find(r => r.Reference == user);
		}

		public string getName()
		{
			return name;
		}

		public ICollection<Message> getMessages()
		{
			return messages.Values;
		}

		public bool matchWithUser(User user)
		{
			if (Users.Contains(user))
				return false;
			users.Add(new Refrence<User>(user));
			return true;
		}

		public bool reMatchWithUser(User user)
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

		internal void applyUpdates(ConversationUpdates conv)
		{
			users.AddRange(conv.getUsersFull());
			List<int> newMssgIDs = new List<int>();
			foreach (var mess in conv.getMessages())
			{
				addMessageUnsafe(mess);
				newMssgIDs.Add(mess.ID);
			}
			foreach (int id in newMssgIDs)
			{
				messages[id].TargetedMessage = messages[id].TargetId == -1 ? null : messages[messages[id].TargetId];
				messages[id].AuthorRef = users.Find(u => u.Reference.Name == messages[id].Author.Name);
			}
		}

		public Message addMessage(User user, int parentID, MessageContent messageContent1, DateTime datetime)
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

		public Message addMessage(Stream stream)
		{
			var formatter = new BinaryFormatter();
			Message mess = (Message)formatter.Deserialize(stream);
			if (mess != null)
			{
				if (mess.ID >= smallestFreeId)
				{
					messages.Add(mess.ID, mess);
					if (mess.TargetId == -1)
					{
						mess.TargetedMessage = null;

					}
					else if (messages.ContainsKey(mess.TargetId))
					{
						mess.TargetedMessage = messages[mess.TargetId];
					}
					else if (mess.TargetId == -1)
					{
						mess.TargetedMessage = null;
					}
					else
					{
						// Cannot find parent message
						return null;
					}
					smallestFreeId = mess.ID;
					return mess;
				}
			}
			return null;
		}

		public Message getMessage(int id)
		{
			if (messages.ContainsKey(id))
				return messages[id];
			return null;
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
			m.TargetedMessage = messages.GetValueOrDefault(m.TargetId, null);
			return result ? m : null;
		}

		/// <summary>
		/// Fixes the internal structure of messages
		/// </summary>
		public void converge()
		{
			foreach (Message message in messages.Values)
			{
				message.TargetedMessage = messages.GetValueOrDefault(message.TargetId, null);
			}
		}

		public bool unmatchWithUser(User user)
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

		public MemoryStream serialize()
		{
			MemoryStream stream = new MemoryStream();
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, this);
			stream.Flush();
			stream.Position = 0;
			return stream;
		}

		public ConversationUpdates getUpdates(int lastMessageId)
		{
			var lastMessageTime = getMessage(lastMessageId)?.getTime();
			return getUpdates(lastMessageTime);
		}

		public ConversationUpdates getUpdates(DateTime? time)
		{
			var updates = new ConversationUpdates(Name, ID);

			updates.Users = getUsers();

			foreach (var message in messages.Values)
			{
				if (message.getTime() > time)
				{
					updates.addMessageUnsafe(new Message(message));
				}
			}
			updates.converge();
			return updates;
		}
	}
}