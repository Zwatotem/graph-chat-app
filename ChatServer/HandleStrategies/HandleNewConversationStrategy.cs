using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies;

/// <summary>
/// Class handling request to create new conversation.
/// </summary>
class HandleNewConversationStrategy : IHandleStrategy
{
	public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem,
		IClientHandler handlerThread, byte[] messageBytes)
	{
		Console.WriteLine("DEBUG: {0} request received", "add new conversation");
		//decoding request - first 4 bytes are length of proposed conversation name, next there is the name
		List<string> namesOfParticipants = new List<string>();
		int index = 0;
		int stringLength = BitConverter.ToInt32(messageBytes, 0);
		index += 4;
		string proposedConversationName = Encoding.UTF8.GetString(messageBytes, index, stringLength);
		index += stringLength;
		//the rest of the request are some number of pairs: (4 bytes describing length of user name, name of user to add to conversation)
		while (index < messageBytes.Length)
		{
			stringLength = BitConverter.ToInt32(messageBytes, index);
			index += 4;
			namesOfParticipants.Add(Encoding.UTF8.GetString(messageBytes, index, stringLength));
			index += stringLength;
		}

		Console.WriteLine("DEBUG: trying to add conversation");
		byte[] reply = new byte[1];
		lock (allHandlers)
		{
			Conversation newConversation =
				chatSystem.AddConversation(proposedConversationName, namesOfParticipants.ToArray());
			if (newConversation == null)
			{
				reply[0] = 0; //conversation could not be created
			}
			else
			{
				reply[0] = 1;
				ConversationUpdates conversationUpdates = newConversation.GetUpdates(DateTime.MinValue);
				byte[] msg = conversationUpdates.Serialize(new ConcreteSerializer()).ToArray();
				//if conversation created successfully, send it to all participating users that are currently connected
				foreach (var handler in allHandlers.FindAll(h => namesOfParticipants.Contains(h.HandledUserName)))
				{
					handler.sendMessage(5, msg);
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