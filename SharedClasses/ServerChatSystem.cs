using System;
using System.Collections.Generic;

namespace ChatModel;

/// <summary>
/// Concrete instance of IServerChatSystem.
/// </summary>
public class ServerChatSystem : ChatSystem, IServerChatSystem
{
	public ServerChatSystem() : base() { }

	public UserUpdates getUpdatesToUser(string userName, DateTime t)
	{
		var user = users.Find(u => u.Name == userName);
		if (user == null)
		{
			return null; //cannot be done if there is no such user
		}
		var updates = new UserUpdates();
		foreach (var conv in user.Conversations)
		{
			updates.addConversation(conv.getUpdates(t)); //get updates to all of users conversations
		}
		return updates;
	}

	public List<Conversation> getConversationsOfUser(string userName)
	{
		IUser user = getUser(userName);
		if (user == null)
		{
			return null; //if there is no such user, return null
		}
		return user.Conversations;
	}
}

/*
This class complies with Liskov Substitution as it properly implements all base and interface methods. Its only responsibility is to realize
chat system logic necessary only on the server side of the app.
*/