using System;
using System.Collections.Generic;
using ChatModel.Util;

namespace ChatModel
{
	[Serializable]
    public abstract class BaseConversation
    {
        protected string name;
        protected int id;
        protected List<Refrence<IUser>> users;
        protected Dictionary<int, Message> messages;

		public string Name { get => name; }

		public int ID { get => id; }

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

		public ICollection<Message> Messages { get => messages.Values; }

		protected BaseConversation() { }

		public BaseConversation(string name, int id)
		{
			this.name = name;
			this.id = id;
			this.users = new List<Refrence<IUser>>();
			this.messages = new Dictionary<int, Message>();
		}

	}
}
