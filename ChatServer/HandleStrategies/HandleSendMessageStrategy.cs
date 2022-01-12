using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
	class HandleSendMessageStrategy : IHandleStrategy
	{
		/// <summary>
		/// Class handling request to send message to conversation.
		/// </summary>
		public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem,
			IClientHandler handlerThread, byte[] messageBytes)
		{
			Console.WriteLine("DEBUG: {0} request received", "send message");
			//decoding request - first 4 bytes are id of conversation, next 4 are id of message to which we reply the rest are message content bytes
			Message message = new Message(new MemoryStream(messageBytes), new ConcreteDeserializer());
			Console.WriteLine("DEBUG: trying to send message");
			byte[] reply = new byte[1];
			lock (allHandlers)
			{
				Message sentMessage =
					chatSystem.SendMessage(
						message.ConversationId,
						message.AuthorID,
						message.TargetId,
						message.Content,
						DateTime.Now);
				if (sentMessage != null)
				{
					//if successful conversation id with serialized message are broadcasted to all connected users from this conversation
					var conversationId = sentMessage.ConversationId;
					reply[0] = 1;
					byte[] msg = sentMessage.Serialize(new ConcreteSerializer()).ToArray();
					var deserializedMessage = new Message(new MemoryStream(msg), new ConcreteDeserializer());
					Conversation conversation = chatSystem.GetConversation(conversationId);
					foreach (var handler in allHandlers.FindAll(h =>
								conversation.Users.Any(u => u.Name == h.HandledUserName)))
					{
						handler.sendMessage(6, msg); //sent message - type 6
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
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/