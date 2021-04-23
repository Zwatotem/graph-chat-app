using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class ClientChatSystemTest
	{
		public void getUserNameTest()
		{
			UserChatSystem chatSystem = new UserChatSystem();
			string name = chatSystem.getUserName();
			Assert.IsNull(name);
			User user1 = chatSystem.addUser("Jaú Kowalski");
			chatSystem.logIn("Jaú Kowalski");
			name = chatSystem.getUserName();
			Assert.IsTrue("Jaú Kowalski" == name);
		}

		public void applyUpdatesTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);

			ClientChatSystemTest clientChatSystem = new ClientChatSystemTest();
			clientChatSystem.applyUpdates(chatSystem.getUpdatesOfUser("Kasia èdüb≥o"));
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getConversationsOfUser("Kasia èdüb≥o"))
			{
				if (conversation.getId() == savedConversation.getId())
				{
					conversationPresent = true;
					bool messagePresent = false;
					foreach (var message in conversation.getMessages())
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

		public void logInTest()
		{
			ClientChatSystemTest chatSystem = new ClientChatSystemTest();
			Assert.IsFalse(chatSystem.logInTest("Kasia èdüb≥o");
			chatSystem.addNewUser("Kasia èdüb≥o");
			Assert.IsTrue(chatSystem.logInTest("Kasia èdüb≥o");
			Assert.IsTrue(chatSystem.getUserName() == "Kasia èdüb≥o");
		}
	}
}