using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace chatAppTest
{
	[TestClass]
	public class ClientChatSystemTest
	{
		[TestMethod]
		public void getUserNameTest()
		{
			IClientChatSystem chatSystem = new ClientChatSystem();
			string name = chatSystem.LoggedInName;
			Assert.IsNull(name);
			IUser user1 = chatSystem.addNewUser("Jaú Kowalski");
			chatSystem.logIn("Jaú Kowalski");
			name = chatSystem.LoggedInName;
			Assert.IsTrue(name == "Jaú Kowalski");
		}

		[TestMethod]
		public void applyUpdatesTest()
		{
			// Creating 'server' chat system
			IServerChatSystem chatSystem = new ServerChatSystem();
			// Creating two users
			IUser user1 = chatSystem.addNewUser("Jaú Kowalski");
			IUser user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			// Creating a conversation with those users
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			// Sending a message
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.ID, "Jaú Kowalski", -1, msgContent1, datetime);
			// Creating client chat system
			IClientChatSystem clientChatSystem = new ClientChatSystem();
			clientChatSystem.addNewUser("Kasia èdüb≥o");
			// Applying updates
			clientChatSystem.applyUpdates(chatSystem.getUpdatesToUser("Kasia èdüb≥o", datetime - TimeSpan.FromSeconds(3)));
			// Checks
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getUser("Kasia èdüb≥o").Conversations)
			{
				if (conversation.ID == savedConversation.ID)
				{
					conversationPresent = true;
					bool messagePresent = false;
					foreach (var message in conversation.Messages)
					{
						if (message.ID == sentMessage1.ID && message.Content.getData() == sentMessage1.Content.getData())
						{
							messagePresent = true;
						}
					}
					Assert.IsTrue(messagePresent);
				}
			}
			Assert.IsTrue(conversationPresent);
		}

		[TestMethod]
		public void logInTest()
		{
			IClientChatSystem chatSystem = new ClientChatSystem();
			Assert.IsFalse(chatSystem.logIn("Kasia èdüb≥o"));
			chatSystem.addNewUser("Kasia èdüb≥o");
			Assert.IsTrue(chatSystem.logIn("Kasia èdüb≥o"));
			Assert.IsTrue(chatSystem.LoggedInName == "Kasia èdüb≥o");
		}
	}
}