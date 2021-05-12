using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel
{
	[Serializable]
	public class Conversation
	{
		private string name;
		private int id;
		private List<User> users;
		private Dictionary<int, Message> messages;
		private int smallestFreeId;

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
			this.users = new List<User>();
			this.messages = new Dictionary<int, Message>();
			this.smallestFreeId = 1;
		}

		public int getId()
		{
			return id;
		}

		public List<User> getUsers()
		{
			return users;
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
			if (users.Contains(user))
				return false;
			users.Add(user);
			return true;
		}

		public Message addMessage(User user, int parentID, MessageContent messageContent1, DateTime datetime)
		{
			int newID = smallestFreeId;
			if ((messages.ContainsKey(parentID) || parentID == -1) && users.Contains(user) && !messages.ContainsKey(newID))
			{
				Message message = new Message(user, parentID == -1 ? null : messages[parentID], messageContent1, datetime, newID);
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

		public bool unmatchWithUser(User user)
		{
			if (users.Contains(user))
			{
				users.Remove(user);
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

		public Conversation getUpdates(int lastMessageId)
		{
			throw new NotImplementedException();
		}
	}
}