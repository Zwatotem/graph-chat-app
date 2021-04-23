using System;
using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			User user1 = chatSystem.addUser("Jaú Kowalski");
			chatSystem.logIn("Jaú Kowalski");
			name = chatSystem.getUserName();
			Assert.IsTrue("Jaú Kowalski" == name);
		}

		[TestMethod]
		public void applyUpdatesTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			MessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);

			ClientChatSystem clientChatSystem = new ClientChatSystem();
			clientChatSystem.applyUpdates(chatSystem.getUpdatesOfUser("Kasia èdüb≥o"));
			bool conversationPresent = false;
			foreach (Conversation conversation in clientChatSystem.getConversationsOfUser("Kasia èdüb≥o"))
			{
				if (conversation.getId() == savedConversation.getId())
				{
					conversationPresent = true;
					bool messagePresent = false;
					foreach (Message message in conversation.getMessages())
					{
						if (message.getId() == sentMessage1.getId() && message.getContent().getData() == sentMessage1.getContent().getData())
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
			Assert.IsFalse(chatSystem.logInTest("Kasia èdüb≥o"));
			chatSystem.addNewUser("Kasia èdüb≥o");
			Assert.IsTrue(chatSystem.logInTest("Kasia èdüb≥o"));
			Assert.IsTrue(chatSystem.getUserName() == "Kasia èdüb≥o");
		}
	}
}