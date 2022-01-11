using ChatModel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ChatModel;

/// <summary>
/// Concrete implementation of IChatSystem.
/// </summary>
/// <remarks>Abstract as it was specified to be so in documentation.</remarks>
public abstract class ChatSystem : IChatSystem, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged = (obj, e) => { };
	protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged(sender, args);

	protected Dictionary<Guid, Conversation>
		conversations; //dictionary of all conversations in the chat system, indexed by their unique id

	protected Dictionary<Guid, IUser> users; //list of all users in the chat system, each has an unique user name

	public IEnumerable<IUser> Users
	{
		get => users.Select(x => x.Value);
		set => users = value.ToDictionary(u => u.ID, u => u);
	}

	public Dictionary<Guid, Conversation> Conversations
	{
		get => conversations;
	}

	public ObservableCollection<Conversation> ObservableConversations
	{
		get
		{
			var oc = new ObservableCollection<Conversation>();
			foreach (var c in conversations)
			{
				oc.Add(c.Value);
			}

			return oc;
		}
	}

	public ChatSystem()
	{
		this.conversations = new Dictionary<Guid, Conversation>();
		this.users = new Dictionary<Guid, IUser>();
	}

	public IUser GetUser(string userName)
	{
		//returns first found user with specific name (there's at most one as names are unique)
		return users
			.FirstOrDefault(kvp => kvp.Value.Name == userName, new KeyValuePair<Guid, IUser>(Guid.Empty, null))
			.Value;
	}

	public IUser AddNewUser(string newUserName)
	{
		if (users.Any(u => u.Value.Name == newUserName)) //checking if the proposed user name would be unique
		{
			return null;
		}
		else
		{
			IUser newUser = new User(newUserName, this);
			users.Add(newUser.ID, newUser);
			return newUser;
		}
	}

	public Conversation GetConversation(Guid id)
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

	public Conversation AddConversation(string conversationName, params string[] ownersNames)
	{
		//creates an array to be filled with references to the new conversation's users
		IUser[] owners = new User[ownersNames.Length];
		int index = 0; //index of first free position in the array
		foreach (var userName in ownersNames) //finding all users in a loop
		{
			//finds user with a specific name
			IUser userReference = GetUser(userName);
			if (userReference == null) //if there's no such user conversation cannot be created
			{
				return null;
			}

			owners[index++] = userReference; //if user found, stores the reference in the array
		}

		return AddConversation(conversationName, owners); //calling overloaded method to do next steps
	}

	public Conversation AddConversation(string conversationName, params IUser[] owners)
	{
		Conversation newConversation = new Conversation(conversationName);
		newConversation.ChatSystem = this;
		conversations.Add(newConversation.ID, newConversation);
		foreach (var owner in owners)
		{
			newConversation.MatchWithUser(owner);
			owner.MatchWithConversation(newConversation);
		}

		PropertyChanged(this, new(nameof(Conversations)));
		PropertyChanged(this, new(nameof(ObservableConversations)));
		return newConversation;
	}

	public bool AddUserToConversation(string userName, Guid id)
	{
		IUser userToAdd = GetUser(userName);
		if (userToAdd == null)
		{
			return false; //if there is no such user, indicate failure of the operation
		}

		Conversation conversationToAdd = GetConversation(id);
		if (conversationToAdd == null)
		{
			return false; //if there is no such conversation, indicate failure of the operation
		}

		userToAdd.MatchWithConversation(conversationToAdd); //assigning the conversation to the user
		//assigning the user to the conversation. If they are already assigned
		//a false value is returned
		return conversationToAdd.MatchWithUser(userToAdd);
	}

	public bool LeaveConversation(string userName, Guid id)
	{
		IUser userToRemove = GetUser(userName);
		if (userToRemove == null)
		{
			return false; //if there is no such user, indicate failure of the operation
		}

		Conversation conversation = GetConversation(id);
		if (conversation == null)
		{
			return false; //if there is no such conversation, indicate failure of the operation
		}

		userToRemove.UnmatchWithConversation(conversation); //removes the conversation from user
		bool result = conversation.UnMatchWithUser(userToRemove); //and user from conversation
		if (!result) //if the user was not assigned to the conversation in the first place
		{
			return false; //indicate failure of the operation
		}

		if (!conversation.Users.Any()) //if there would be no users in the conversation left
		{
			conversations.Remove(id); //deletes the conversation
		}

		return true;
	}


	public Message? SendMessage(Guid convId, string userName, Guid targetId, IMessageContent messageContent,
		DateTime sentTime)
	{
		Conversation conversation = FindConversation(convId);
		if (conversation == null)
		{
			return null; //if there is no such conversation, indicate failure of the operation
		}

		IUser author = conversation.Users.FirstOrDefault(u => u.Name == userName, null);
		if (author == null)
		{
			return null; //if there is no such user, indicate failure of the operation
		}

		//calls the conversation method responsible for
		//creating a message. Returns it's returned value. True if operation successful, else false.
		return conversation.AddMessage(author, targetId, messageContent, sentTime);
	}

	public Conversation FindConversation(Guid id)
	{
		return id != Guid.Empty ? conversations[id] : null;
	}

	public IUser FindUser(Guid id)
	{
		return id != Guid.Empty ? users[id] : null;
	}
}

/*
This class is seemingly too big for modern standards, but project documentation and diagrams from business analysis department forced it to be 
implemented that way. Nevertheless: comliant with Liskov Substitution as it properly implements all interface methods, realizes dependency inversion
by referencing IUser rather that user and has no responsibility other than implementing logic necessary for handling of users and conversations.
*/