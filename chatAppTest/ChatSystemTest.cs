using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class ChatSystemTest
	{
		public void AddNewUserTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User savedUser = chatSystem.addNewUser("Jaú Kowalski");
			Assert.IsNotNull(savedUser);

			savedUser = chatSystem.addNewUser("Jaú Kowalski");
			Assert.IsNull(savedUser);
		}

		public void AddConversationTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			User user3 = chatSystem.addUser("PszczÛ≥ka Maja");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation1);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation2);
			Assert.AreNotEqual(savedConversation1.getId(), savedConversation2.getId());
		}

		public void AddUserToConversation()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addUserToConversation("PszczÛ≥ka Maja");
			ICollection collection = user3.getConversations();
			bool isThere = false;
			foreach (var c in collection)
			{
				if (c == savedConversation1)
				{
					isThere = true;
				}
			}
			Assert.IsTrue(isThere);

			collection = savedConversation1.getUsers();
			isThere = false;
			foreach (var u in collection)
			{
				if (u == user3)
				{
					isThere = true;
				}
			}
			Assert.IsTrue(isThere);
		}

		public void LeaveConversation()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			chatSystem.leaveConversation("Kasia èdüb≥o", savedConversation1.getId());
			ICollection users = savedConversation1.getUsers();
			foreach (var u in users)
			{
				Assert.IsFalse(u == user2);
			}
			ICollection conversations = user2.getConversations();
			foreach (var c in conversations)
			{
				Assert.IsFalse(c == savedConversation1);
			}
		}

		public void GetConversationTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation returnedConversation = chatSystem.getConversation(savedConversation.getId());
			Assert.IsTrue(returnedConversation == savedConversation);
		}

		public void sendMessageTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;  //do poprawy na jakiú legitny typ
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.getParent());
			Content msgContent2 = new TextContent("CzeúÊ");
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia èdüb≥o", sentMessage1.getId(), msgContent2, datetime);
			Assert.IsNotNull(sentMessage2);
			Assert.IsTrue(sentMessage2.getParent() == sentMessage1); //test comment
		}
	}
}