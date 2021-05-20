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
			ClientChatSystem chatSystem = new ClientChatSystem();
			string name = chatSystem.getUserName();
			Assert.IsNull(name);
			User user1 = chatSystem.addNewUser("Ja� Kowalski");
			chatSystem.logIn("Ja� Kowalski");
			name = chatSystem.getUserName();
			Assert.IsTrue(name == "Ja� Kowalski");
		}

		[TestMethod]
		public void applyUpdatesTest()
		{
			// Creating 'server' chat system
			ServerChatSystem chatSystem = new ServerChatSystem();
			// Creating two users
			User user1 = chatSystem.addNewUser("Ja� Kowalski");
			User user2 = chatSystem.addNewUser("Kasia �d�b�o");
			// Creating a conversation with those users
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			// Sending a message
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.ID, "Ja� Kowalski", -1, msgContent1, datetime);
			// Creating client chat system
			ClientChatSystem clientChatSystem = new ClientChatSystem();
			clientChatSystem.addNewUser("Kasia �d�b�o");
			// Applying updates
			clientChatSystem.applyUpdates(chatSystem.getUpdatesToUser("Kasia �d�b�o", datetime - TimeSpan.FromSeconds(3)));
			// Checks
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getUser("Kasia �d�b�o").getConversations())
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
			ClientChatSystem chatSystem = new ClientChatSystem();
			Assert.IsFalse(chatSystem.logIn("Kasia �d�b�o"));
			chatSystem.addNewUser("Kasia �d�b�o");
			Assert.IsTrue(chatSystem.logIn("Kasia �d�b�o"));
			Assert.IsTrue(chatSystem.getUserName() == "Kasia �d�b�o");
		}
	}
}