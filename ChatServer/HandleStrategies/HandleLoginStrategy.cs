using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies;

class HandleLoginStrategy : IHandleStrategy
{
	/// <summary>
	/// Class handling request to log in.
	/// </summary>
	public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem,
		IClientHandler handlerThread, byte[] messageBytes)
	{
		Console.WriteLine("DEBUG: {0} request received", "logIn");
		//decoding request - all bytes are user name under which they want to log in
		string userName = Encoding.UTF8.GetString(messageBytes);
		Console.WriteLine("DEBUG: requested logIn");
		byte[] reply = new byte[1024*256];
		lock (allHandlers)
		{
			IUser user = chatSystem.GetUser(userName);
			if (handlerThread.HandledUserName != null || user == null ||
				allHandlers.Exists(h => h.HandledUserName == userName))
			{
				//if this client is already logged in or there is no user with this user name or this user is already logged in
				reply[0] = 0; //indicate that log in failed
			}
			else
			{
				reply[0] = 1;
				handlerThread.HandledUserName = userName;
				// Send serialized User, so that the client can know the user's name and id
				var serializedUser = (chatSystem.GetUser(userName) as User)
					.Serialize(new ConcreteSerializer()).ToArray();
				Array.Copy(serializedUser, 0,reply, 1, serializedUser.Length);
				//if login successful, send to this client all conversations in which this user takes part.
				foreach (var conversation in user.Conversations)
				{
					byte[] msg = conversation.Serialize(new ConcreteSerializer()).ToArray();
					handlerThread.sendMessage(5, msg);
				}
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