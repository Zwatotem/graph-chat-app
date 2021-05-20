using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace chatAppTest
{
	[TestClass]
	public class ServerChatSystemTest
	{
		[TestMethod]
		public void getUpdatesToUserTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Ja� Kowalski");
			User user2 = chatSystem.addNewUser("Kasia �d�b�o");
			User user3 = chatSystem.addNewUser("Roch Kowal");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user1, user3);
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			IMessageContent msgContent2 = new TextContent("Heeej");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.ID, "Ja� Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation2.ID, "Ja� Kowalski", -1, msgContent2, datetime);
			UserUpdates updates = chatSystem.getUpdatesToUser("Kasia �d�b�o", datetime - TimeSpan.FromSeconds(3));
			bool containsConversation = false;
			bool containsWrongConversation = false;
			foreach (var conversation in updates)
			{
				if (conversation.ID == savedConversation.ID && conversation.Name == savedConversation.Name)
				{
					containsConversation = true;
					bool containsMessage = false;
					bool containsWrongMessage = false;
					foreach (var message in conversation.Messages)
					{
						if (message.ID == sentMessage1.ID)
						{
							containsMessage = true;
							Assert.IsTrue(message.Author.getName() == sentMessage1.Author.getName());
							Assert.IsNull(message.Parent);
							Assert.IsTrue(message.Content.getData() == sentMessage1.Content.getData());
							Assert.IsTrue(message.SentTime == sentMessage1.SentTime);
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
			User user1 = chatSystem.addNewUser("Ja� Kowalski");
			User user2 = chatSystem.addNewUser("Kasia �d�b�o");
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
			foreach (var conversation in chatSystem.getConversationsOfUser("Ja� Kowalski"))
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
			foreach (var conversation in chatSystem.getConversationsOfUser("Kasia �d�b�o"))
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