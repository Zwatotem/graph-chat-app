using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace ChatModel
{
	/// <summary>
	/// Concrete implementation of IChatSystem.
	/// </summary>
	/// <remarks>Abstract as it was specified to be so in documentation.</remarks>
	public abstract class ChatSystem : IChatSystem, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (obj, e) => { };
		
		protected Dictionary<int, Conversation> conversations; //dictionary of all conversations in the chat system, indexed by their unique id
		protected List<IUser> users; //list of all users in the chat system, each has an unique user name
		protected int smallestFreeId; //smallest unique id available to be assigned to a new conversation
		protected Stack<int> freedIds; //stack of conversation ids smaller than current smallest available that were freed be deleting conversations

		public List<IUser> Users { get => users; }
		public Dictionary<int, Conversation> Conversations { get => conversations; }
		public ObservableCollection<Conversation> observableConversations
		{
			get
			{
				var oc = new ObservableCollection<Conversation>();
				foreach(var c in conversations)
				{
					oc.Add(c.Value);
				}
				return oc;
			}
		}
		
		public ChatSystem()
		{
			this.conversations = new Dictionary<int, Conversation>();
			this.users = new List<IUser>();
			this.smallestFreeId = 1;
			this.freedIds = new Stack<int>();
		}

		public IUser getUser(string userName)
		{
			return users.Find(u => u.Name == userName); //returns first found user with specific name (there's at most one as names are unique)
		}

		public IUser addNewUser(string newUserName)	
		{
			if (users.Exists(u => u.Name == newUserName)) //checking if the proposed user name would be unique
			{ 
				return null;
			}
			else
			{
				IUser newUser = new User(newUserName);
				users.Add(newUser);
				return newUser;
			}
		}

		public Conversation getConversation(int id)
		{
			if (conversations.ContainsKey(id))
			{
				return conversations[id];
			}
			else
			{
				return null;
			}
		}

		public Conversation addConversation(string conversationName, params string[] ownersNames)
		{
			IUser[] owners = new User[ownersNames.Length]; //creates an array to be filled with references to the new conversation's users
			int index = 0; //index of first free position in the array
			foreach (var userName in ownersNames) //finding all users in a loop
			{
				IUser userReference = users.Find(u => u.Name == userName); //finds user with a specific name
				if (userReference == null) //if there's no such user conversation cannot be created
				{
					return null;
				}
				else
				{
					owners[index++] = userReference; //if user found stores the reference in the array
				}
			}
			return addConversation(conversationName, owners); //calling overloaded method to do next steps
		}

		public Conversation addConversation(string conversationName, params IUser[] owners)
		{
			int newId;
			if (freedIds.Count > 0) //if there are any freed ids on the stack, one of the is going to be reused
			{
				newId = freedIds.Pop();
			}
			else
			{
				newId = smallestFreeId++; //else we take current smallest available id and set smallestFreeId to next integer
			}
			foreach (var owner in owners) //check if all owners are indeed part of the chat system
            {
				if (!users.Contains(owner))
                {
					return null;
                }
            }
			Conversation newConversation = new Conversation(conversationName, newId);
			conversations.Add(newId, newConversation);
			foreach (var owner in owners)
			{
				newConversation.matchWithUser(owner);
				owner.matchWithConversation(newConversation);
			}
			return newConversation;
		}

		public Conversation addConversation(Stream stream)
		{
			throw new NotImplementedException();
		}

		public bool addUserToConversation(string userName, int id)
		{
			IUser userToAdd = getUser(userName);
			if (userToAdd == null)
			{
				return false; //if there is no such user, indicate failure of the operation
			}
			Conversation conversationToAdd = getConversation(id);
			if (conversationToAdd == null)
			{
				return false; //if there is no such conversation, indicate failure of the operation
			}
			userToAdd.matchWithConversation(conversationToAdd); //assigning the conversation to the user
			return conversationToAdd.matchWithUser(userToAdd); //assigning the user to the conversation. If they are already assigned
															   //a false value is returned
		}

		public bool leaveConversation(string userName, int id)
		{
			IUser userToRemove = getUser(userName);
			if (userToRemove == null)
			{
				return false; //if there is no such user, indicate failure of the operation
			}
			Conversation conversation = getConversation(id);
			if (conversation == null)
			{
				return false; //if there is no such conversation, indicate failure of the operation
			}
			userToRemove.unmatchWithConversation(conversation); //removes the conversation from user
			bool result = conversation.unmatchWithUser(userToRemove); //and user from conversation
			if (!result) //if the user was not assigned to the conversation in the first place
			{
				return false; //indicate failure of the operation
			}
			else
			{
				if (conversation.Users.Count == 0) //if there would be no users in the conversation left
				{
					conversations.Remove(id); //deletes the conversation
					if (id == smallestFreeId - 1) //if the id of deleted conversation was only one smaller than smallestFreeId
					{
						smallestFreeId--; //decrement the variable
					}
					else
					{
						freedIds.Push(id); //push the id to the stack for reuse
					}
				}
				return true;
			}
		}


		public Message sendMessage(int convId, string userName, int targetId, IMessageContent messageContent, DateTime sentTime)
		{
			Conversation conversation = getConversation(convId);
			if (conversation == null)
			{
				return null; //if there is no such conversation, indicate failure of the operation
			}
			IUser author = conversation.Users.Find(u => u.Name == userName);
			if (author == null)
			{
				return null; //if there is no such user, indicate failure of the operation
			}
			return conversation.addMessage(author, targetId, messageContent, sentTime); //calls the conversation method responsible for
																						 //creating a message. Returns it's returned value. True if operation successful, else false.
		}
	}
}

/*
This class is seemingly too big for modern standards, but project documentation and diagrams from business analysis department forced it to be 
implemented that way. Nevertheless: comliant with Liskov Substitution as it properly implements all interface methods, realizes dependency inversion
by referencing IUser rather that user and has no responsibility other than implementing logic necessary for handling of users and conversations.
*/