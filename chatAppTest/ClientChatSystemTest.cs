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
			string name = chatSystem.LoggedUserName;
			Assert.IsNull(name);
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			chatSystem.logIn(new User("Jaś Kowalski", chatSystem as ChatSystem));
			name = chatSystem.LoggedUserName;
			Assert.IsTrue(name == "Jaś Kowalski");
		}

		[TestMethod]
		public void applyUpdatesTest()
		{
			// Creating 'server' chat system
			IServerChatSystem chatSystem = new ServerChatSystem();
			// Creating two users
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			// Creating a conversation with those users
			Conversation savedConversation = chatSystem.AddConversation("Konfa 1", user1, user2);
			// Sending a message
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.SendMessage(savedConversation.ID, "Jaś Kowalski", Guid.Empty, msgContent1, datetime);
			// Creating client chat system
			IClientChatSystem clientChatSystem = new ClientChatSystem();
			clientChatSystem.AddNewUser("Kasia Źdźbło");
			// Applying updates
			clientChatSystem.applyUpdates(chatSystem.getUpdatesToUser("Kasia Źdźbło", datetime - TimeSpan.FromSeconds(3)));
			// Checks
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.GetUser("Kasia Źdźbło").Conversations)
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
			Assert.IsFalse(chatSystem.logIn(new User("Kasia Źdźbło", chatSystem as ChatSystem)));
			chatSystem.AddNewUser("Kasia Źdźbło");
			Assert.IsTrue(chatSystem.logIn(new User("Kasia Źdźbło", chatSystem as ChatSystem)));
			Assert.IsTrue(chatSystem.LoggedUserName == "Kasia Źdźbło");
		}
	}
}