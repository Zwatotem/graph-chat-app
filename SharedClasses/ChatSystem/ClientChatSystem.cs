using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Concrete implementation of IClientChatSystem.
/// </summary>
public class ClientChatSystem : ChatSystem, IClientChatSystem
{
	private Guid loggedUserID; //name of logged in user

	public IUser LoggedUser
	{
		get => users[loggedUserID];
	}

	public ClientChatSystem() : base()
	{
		this.loggedUserID = Guid.Empty; //indicates that there is no logged in user at the start
	}

	public string LoggedUserName
	{
		get => LoggedUser.Name;
	}

	public bool logIn(IUser user)
	{
		//if no one is logged in and the user exists
		if (loggedUserID == Guid.Empty)
		{
			if (users.ContainsKey(user.ID))
			{
				if (users[user.ID].Equals(user))
				{
					loggedUserID = user.ID;
					return true;
				}
				return false;
			}
			users.Add(user.ID, user);
			loggedUserID = user.ID;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets name of logged in user.
	/// </summary>
	/// <returns>the user name of the user currently logged in or null if there is no such user</returns>

	public Conversation AddConversation(Stream stream, IDeserializer deserializer)
	{
		Conversation conv = (Conversation)deserializer.Deserialize(stream);
		if (conversations.ContainsKey(conv.ID))
		{
			return null; //if there is already a conversation with this id
		}

		var newUsers = new List<IUser>(); // List of overlapping IUser objects
		Users = Users.Union(newUsers);

		foreach (var user in newUsers)
		{
			//fix references in the conversation that point to users already present in the system so that they point to correct objects
			user.MatchWithConversation(conv);
		}

		conversations.Add(conv.ID, conv); //add the conversation to the chat system
		OnPropertyChanged(this, new(nameof(Conversations)));
		OnPropertyChanged(this, new(nameof(ObservableConversations)));
		return conv;
	}

	public void applyUpdates(UserUpdates updates)
	{
		foreach (var convUpdate in updates)
		{
			if (!conversations.ContainsKey(convUpdate.ID)) //if there is no such conversation we add it
			{
				var newUsers = new List<IUser>(); // List of overlapping IUser objects
				conversations.Add(convUpdate.ID, new Conversation(convUpdate));
				Users = Users.Union(newUsers);

				foreach (var user in newUsers)
				{
					//fix references in the conversation that point to users already present in the system so that they point to correct objects
					user.MatchWithConversation(conversations[convUpdate.ID]);
				}
			}
			else //else we update the one already present
			{
				conversations[convUpdate.ID].ApplyUpdates(convUpdate);
			}
		}
	}
}

/*
Class complies with Liskov Substitution as it properly implements all interface and base methods. Its only resposibility is chat system logic
necessary only on the client side of the app.
*/