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
			ChatSystem chatSystem = new ServerChatSystem();
			User savedUser = chatSystem.addNewUser("Jaú Kowalski");
			Assert.IsNotNull(savedUser);

			savedUser = chatSystem.addNewUser("Jaú Kowalski");
			Assert.IsNull(savedUser);
		}

		[TestMethod]
		public void AddConversationTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			User user3 = chatSystem.addNewUser("PszczÛ≥ka Maja");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation1);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation2);
			Assert.AreNotEqual(savedConversation1.getId(), savedConversation2.getId());
		}

		[TestMethod]
		public void AddUserToConversation()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addNewUser("PszczÛ≥ka Maja");
			chatSystem.addUserToConversation("PszczÛ≥ka Maja", savedConversation1.getId());
			var collection1 = user3.getConversations();
			bool isThere = false;
			foreach (var c in collection1)
			{
				if (c == savedConversation1)
				{
					isThere = true;
				}
			}
			Assert.IsTrue(isThere);

			var collection2 = savedConversation1.getUsers();
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
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			chatSystem.leaveConversation("Kasia èdüb≥o", savedConversation1.getId());
			var users = savedConversation1.getUsers();
			foreach (var u in users)
			{
				Assert.IsFalse(u == user2);
			}
			var conversations = user2.getConversations();
			foreach (var c in conversations)
			{
				Assert.IsFalse(c == savedConversation1);
			}
		}

		[TestMethod]
		public void GetConversationTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation returnedConversation = chatSystem.getConversation(savedConversation.getId());
			Assert.IsTrue(returnedConversation == savedConversation);
		}

		[TestMethod]
		public void sendMessageTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			MessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.getParent());
			Assert.IsTrue(sentMessage1.getUser() == user1);
			Assert.IsTrue(sentMessage1.getContent() == msgContent1);
			Assert.IsTrue(sentMessage1.getTime() == datetime);
			MessageContent msgContent2 = new TextContent("CzeúÊ");
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia èdüb≥o", sentMessage1.getId(), msgContent2, datetime);
			Assert.IsNotNull(sentMessage2);
			Assert.IsTrue(sentMessage2.getParent() == sentMessage1);
			Assert.IsTrue(sentMessage2.getUser() == user2);
			Assert.IsTrue(sentMessage2.getContent() == msgContent2);
			Assert.IsTrue(sentMessage2.getTime() == datetime);
		}
	}
}