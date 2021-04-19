using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

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
			foreach(var c in collection)
			{
				if(c == savedConversation1)
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
			foreach(var c in conversations)
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
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.getParent());
			Content msgContent2 = new TextContent("CzeúÊ");
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia èdüb≥o", sentMessage1.getId(), msgContent2);
			Assert.IsNotNull(sentMessage2);
			Assert.IsTrue(sentMessage2.getParent() == sentMessage1); //test comment
		}
	}

	[TestClass]
	public class ServerChatSystemTest
	{
		public void getUpdatesToUserTest()
        {
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			User user3 = chatSystem.addUser("Roch Kowal");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user1, user3);
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("Heeej");
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation2.getId(), "Jaú Kowalski", -1, msgContent2);
			ICollection updates = chatSystem.getUpdatesOfUser("Kasia èdüb≥o"); //czy na pewno tak przechowujemy updaty?
			bool containsConversation = false; 
			bool containsWrongConversation = false;
			foreach (var update in updates.getConversationUpdates()) //czy aby tak?
            {
				if (update.getId() == savedConversation.getId())
                {
					containsConversation = true;
					bool containsMessage = false;
					bool containsWrongMessage = false;
					foreach (var message in update.getMessages())
                    {
						if (message.getId() == sentMessage1.getId())
                        {
							containsMessage = true;
                        }
						else
                        {
							containsWrongMessage = true;
                        }
                    }
					Assert.isTrue(containsMessage);
					Assert.isFalse(containsWrongMessage);
                }
				else
                {
					containsWrongConversation = true;
                }
            }
			Assert.isTrue(containsConversation);
			Assert.isFalse(containsWrongConversation);
        }

		public void getConversationsOfUserTest()
        {
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addUser("Johannes von Neustadt");
			Conversation savedConversation2 = chatSystem.addConversation("Ziomki", user1, user3);
			int present = 0;
			foreach (var conversation in chatSystem.getConversationsOfUser("Johannes von Neustadt"))
            {
				if (conversation == savedConversation2)
                {
					present += 2;
                }
				else
                {
					present -= 30;
                }
            }
			Assert.isTrue(present == 2);
			present = 0;
			foreach (var conversation in chatSystem.getConversationsOfUser("Kasia èdüb≥o"))
            {
				if (conversation == savedConversation1)
                {
					present += 1;
                }
				else if (conversation == savedConversation2)
                {
					present += 2;
                }
				else
                {
					present -= 30;
                }
            }
			Assert.isTrue(present == 3);
			present = 0;
			foreach (var conversation in chatSystem.getConversationsOfUser("Jaú Kowalski"))
            {
				if (conversation == savedConversation1)
                {
					present += 1;
                }
				else
                {
					present -= 30;
                }
            }
			Assert.isTrue(present == 1);
        }
	}

	[TestClass]
	public class ClientChatSystemTest
	{
		public void getUserNameTest()
        {
			UserChatSystem chatSystem = new UserChatSystem();
			string name = chatSystem.getUserName();
			Assert.isNull(name);
			User user1 = chatSystem.addUser("Jaú Kowalski");
			chatSystem.logIn("Jaú Kowalski");
			name = chatSystem.getUserName();
			Assert.isTrue("Jaú Kowalski" == name);
        }

		public void applyUpdatesTest()
        {
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1);

			ClientChatSystemTest clientChatSystem = new ClientChatSystemTest();
			clientChatSystem.applyUpdates(chatSystem.getUpdatesOfUser("Kasia èdüb≥o"));
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getConversationsOfUser("Kasiaèdüb≥o"))
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
					Assert.isTrue(messagePresent);
                }
            }
			Assert.isTrue(conversationPresent);
        }

		public void logInTest()
        {
			ClientChatSystemTest chatSystem = new ClientChatSystemTest();
			Assert.isFalse(chatSystem.logInTest("Kasia èdüb≥o");
			chatSystem.addNewUser("Kasia èdüb≥o");
			Assert.isTrue(chatSystem.logInTest("Kasia èdüb≥o");
			Assert.isTrue(chatSystem.getUserName() == "Kasia èdüb≥o");
        }
	}

	[TestClass]
	public class UserTest
	{

	}

	[TestClass]
	public class ConversationTest
	{
		[TestMethod]
		public void TestMethod1()
		{

		}
	}

	[TestClass]
	public class MessageTest
	{
		[TestMethod]
		public void TestMethod1()
		{
		}
	}
	
	[TestClass]
	public class ContentTest
	{
		
	}

	[TestClass]
	public class TextContentTest
	{
		
	}
}
