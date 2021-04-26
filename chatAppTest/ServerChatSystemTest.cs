using System;
using System.Collections;
using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class ServerChatSystemTest
	{
		[TestMethod]
		public void getUpdatesToUserTest() //do przejrzenia czy na pewno tak to robimy
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			User user3 = chatSystem.addNewUser("Roch Kowal");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user1, user3);
			MessageContent msgContent1 = new TextContent("Heeejoooo");
			MessageContent msgContent2 = new TextContent("Heeej");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation2.getId(), "Jaú Kowalski", -1, msgContent2, datetime);
			User updates = chatSystem.getUpdatesOfUser("Kasia èdüb≥o"); //czy na pewno tak przechowujemy updaty?
			bool containsConversation = false;
			bool containsWrongConversation = false;
			foreach (var update in updates.getConversations()) //czy aby tak?
			{
				if (update.getId() == savedConversation.getId() && update.getName() == savedConversation.getName())
				{
					containsConversation = true;
					bool containsMessage = false;
					bool containsWrongMessage = false;
					foreach (var message in update.getMessages())
					{
						if (message.getId() == sentMessage1.getId())
						{
							containsMessage = true;
							Assert.IsTrue(message.getUser().getName() == sentMessage1.getUser().getName());
							Assert.IsNull(message.getParent());
							Assert.IsTrue(message.getContent().getData() == sentMessage1.getContent().getData());
							Assert.IsTrue(message.getTime() == sentMessage1.getTime());
						}
						else
						{
							containsWrongMessage = true;
						}
					}
					Assert.IsTrue(containsMessage);
					Assert.IsFalse(containsWrongMessage);
				}
				else
				{
					containsWrongConversation = true;
				}
			}
			Assert.IsTrue(containsConversation);
			Assert.IsFalse(containsWrongConversation);
		}

		[TestMethod]
		public void getConversationsOfUserTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addNewUser("Johannes von Neustadt");
			Conversation savedConversation2 = chatSystem.addConversation("Ziomki", user1, user3);
			bool hasConversation1 = false;
			bool hasConversation2 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in chatSystem.getConversationsOfUser("Johannes von Neustadt"))
			{
				if (conversation == savedConversation2)
				{
					hasConversation2 = true;
				}
				else
				{
					hasWrongConversation = true;
				}
			}
			Assert.IsTrue(hasConversation2);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in chatSystem.getConversationsOfUser("Jaú Kowalski"))
			{
				if (conversation == savedConversation1)
				{
					hasConversation1 = true;
				}
				else if (conversation == savedConversation2)
				{
					hasConversation2 = true;
				}
				else
				{
					hasWrongConversation = true;
				}
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsTrue(hasConversation2);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in chatSystem.getConversationsOfUser("Kasia èdüb≥o"))
			{
				if (conversation == savedConversation1)
				{
					hasConversation1 = true;
				}
				else
				{
					hasWrongConversation = true;
				}
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsFalse(hasWrongConversation);
		}
	}
}