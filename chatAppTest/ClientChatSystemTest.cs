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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			chatSystem.logIn("Ja� Kowalski");
			name = chatSystem.getUserName();
			Assert.IsTrue("Ja� Kowalski" == name);
		}

		public void applyUpdatesTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1, datetime);

			ClientChatSystemTest clientChatSystem = new ClientChatSystemTest();
			clientChatSystem.applyUpdates(chatSystem.getUpdatesOfUser("Kasia �d�b�o"));
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getConversationsOfUser("Kasia �d�b�o"))
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
			Assert.IsFalse(chatSystem.logInTest("Kasia �d�b�o");
			chatSystem.addNewUser("Kasia �d�b�o");
			Assert.IsTrue(chatSystem.logInTest("Kasia �d�b�o");
			Assert.IsTrue(chatSystem.getUserName() == "Kasia �d�b�o");
		}
	}
}