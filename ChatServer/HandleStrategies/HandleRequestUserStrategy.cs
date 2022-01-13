using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies;

class HandleRequestUserStrategy : IHandleStrategy
{
	/// <summary>
	/// Class handling request to send info about particular user.
	/// </summary>
	public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem,
		IClientHandler handlerThread, byte[] messageBytes)
	{
		Console.WriteLine("DEBUG: {0} request received", "send message");
		//decoding request - first 4 bytes are id of conversation, next 4 are id of message to which we reply the rest are message content bytes
		Guid requestedUserId = new Guid(messageBytes);
		Console.WriteLine("DEBUG: trying to send message");
		byte[] reply = new byte[1];
		lock (allHandlers)
		{
			IUser requestingUser = chatSystem.GetUser(handlerThread.HandledUserName);
			IUser user =
				chatSystem.FindUser(requestedUserId);
			if (user != null)
			{
				// Check if exists at least one common conversation of these two users
				IEnumerable<Conversation> commonConversations =
					chatSystem.getConversationsOfUser(handlerThread.HandledUserName);
				bool canRespond =
					commonConversations.Any(conversation => conversation.Users.Any(u => u.ID == requestedUserId));
				if (canRespond)
				{
					reply[0] = 1;
					byte[] msg = user.Serialize(new ConcreteSerializer()).ToArray();
					handlerThread.sendMessage(7, msg); //sent message - type 7
				}
				else
				{
					reply[0] = 0;
				}
			}
			else
			{
				reply[0] = 0;
			}
		}

		handlerThread.sendMessage(1, reply);
	}
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/