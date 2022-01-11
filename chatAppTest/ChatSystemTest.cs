using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace chatAppTest
{
	[TestClass]
	public class ChatSystemTest
	{
		[TestMethod]
		public void AddNewUserTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser savedUser = chatSystem.AddNewUser("Jaś Kowalski");
			Assert.IsNotNull(savedUser);

			savedUser = chatSystem.AddNewUser("Jaś Kowalski");
			Assert.IsNull(savedUser);
		}

		[TestMethod]
		public void AddConversationTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			IUser user3 = chatSystem.AddNewUser("Pszczółka Maja");
			Conversation savedConversation1 = chatSystem.AddConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation1);
			Conversation savedConversation2 = chatSystem.AddConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation2);
			Assert.AreNotEqual(savedConversation1.ID, savedConversation2.ID);
		}

		[TestMethod]
		public void AddUserToConversation()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			Conversation savedConversation1 = chatSystem.AddConversation("Konfa 1", user1, user2);
			IUser user3 = chatSystem.AddNewUser("Pszczółka Maja");
			chatSystem.AddUserToConversation("Pszczółka Maja", savedConversation1.ID);
			var collection1 = user3.Conversations;
			bool isThere = false;
			foreach (var c in collection1)
			{
				if (c == savedConversation1)
				{
					isThere = true;
				}
			}
			Assert.IsTrue(isThere);

			var collection2 = savedConversation1.Users;
			isThere = false;
			foreach (var u in collection2)
			{
				if (u == user3)
				{
					isThere = true;
				}
			}
			Assert.IsTrue(isThere);
		}

		[TestMethod]
		public void LeaveConversation()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			Conversation savedConversation1 = chatSystem.AddConversation("Konfa 1", user1, user2);
			chatSystem.LeaveConversation("Kasia Źdźbło", savedConversation1.ID);
			var users = savedConversation1.Users;
			foreach (var u in users)
			{
				Assert.IsFalse(u == user2);
			}
			var conversations = user2.Conversations;
			foreach (var c in conversations)
			{
				Assert.IsFalse(c == savedConversation1);
			}
		}

		[TestMethod]
		public void GetConversationTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			Conversation savedConversation = chatSystem.AddConversation("Konfa 1", user1, user2);
			Conversation returnedConversation = chatSystem.GetConversation(savedConversation.ID);
			Assert.IsTrue(returnedConversation == savedConversation);
		}

		[TestMethod]
		public void sendMessageTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			Conversation savedConversation = chatSystem.AddConversation("Konfa 1", user1, user2);
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.SendMessage(savedConversation.ID, "Jaś Kowalski", Guid.Empty, msgContent1, datetime);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.Parent);
			Assert.IsTrue(sentMessage1.Author == user1);
			Assert.IsTrue(sentMessage1.Content == msgContent1);
			Assert.IsTrue(sentMessage1.SentTime == datetime);
			IMessageContent msgContent2 = new TextContent("Cześć");
			Message sentMessage2 = chatSystem.SendMessage(savedConversation.ID, "Kasia Źdźbło", sentMessage1.ID, msgContent2, datetime);
			Assert.IsNotNull(sentMessage2);
			Assert.IsTrue(sentMessage2.Parent == sentMessage1);
			Assert.IsTrue(sentMessage2.Author == user2);
			Assert.IsTrue(sentMessage2.Content == msgContent2);
			Assert.IsTrue(sentMessage2.SentTime == datetime);
		}
	}
}