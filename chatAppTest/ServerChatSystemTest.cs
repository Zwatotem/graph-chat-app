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
			IServerChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			IUser user3 = chatSystem.AddNewUser("Roch Kowal");
			Conversation savedConversation = chatSystem.AddConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.AddConversation("Konfa 2", user1, user3);
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			IMessageContent msgContent2 = new TextContent("Heeej");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.SendMessage(savedConversation.ID, "Jaś Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.SendMessage(savedConversation2.ID, "Jaś Kowalski", -1, msgContent2, datetime);
			UserUpdates updates = chatSystem.getUpdatesToUser("Kasia Źdźbło", datetime - TimeSpan.FromSeconds(3));
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
							Assert.IsTrue(message.Author.Name == sentMessage1.Author.Name);
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
			IServerChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			Conversation savedConversation1 = chatSystem.AddConversation("Konfa 1", user1, user2);
			IUser user3 = chatSystem.AddNewUser("Johannes von Neustadt");
			Conversation savedConversation2 = chatSystem.AddConversation("Ziomki", user1, user3);
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
			foreach (var conversation in chatSystem.getConversationsOfUser("Jaś Kowalski"))
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
			foreach (var conversation in chatSystem.getConversationsOfUser("Kasia Źdźbło"))
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