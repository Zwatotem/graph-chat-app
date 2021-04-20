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
			User savedUser = chatSystem.addNewUser("Ja� Kowalski");
			Assert.IsNotNull(savedUser);

			savedUser = chatSystem.addNewUser("Ja� Kowalski");
			Assert.IsNull(savedUser);
		}

		public void AddConversationTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			User user3 = chatSystem.addUser("Pszcz�ka Maja");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation1);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 1", user1, user2, user3);
			Assert.IsNotNull(savedConversation2);
			Assert.AreNotEqual(savedConversation1.getId(), savedConversation2.getId());
		}

		public void AddUserToConversation()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addUserToConversation("Pszcz�ka Maja");
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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			chatSystem.leaveConversation("Kasia �d�b�o", savedConversation1.getId());
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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation returnedConversation = chatSystem.getConversation(savedConversation.getId());
			Assert.IsTrue(returnedConversation == savedConversation);
		}

		public void sendMessageTest()
		{
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.getParent());
			Content msgContent2 = new TextContent("Cze��");
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia �d�b�o", sentMessage1.getId(), msgContent2);
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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			User user3 = chatSystem.addUser("Roch Kowal");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user1, user3);
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("Heeej");
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation2.getId(), "Ja� Kowalski", -1, msgContent2);
			ICollection updates = chatSystem.getUpdatesOfUser("Kasia �d�b�o"); //czy na pewno tak przechowujemy updaty?
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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			User user3 = chatSystem.addUser("Johannes von Neustadt");
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
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in chatSystem.getConversationsOfUser("Kasia �d�b�o"))
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
			Assert.isTrue(hasConversation1);
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in chatSystem.getConversationsOfUser("Ja� Kowalski"))
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
			Assert.isTrue(hasConversation1);
			Assert.isFalse(hasWrongConversation);
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
			User user1 = chatSystem.addUser("Ja� Kowalski");
			chatSystem.logIn("Ja� Kowalski");
			name = chatSystem.getUserName();
			Assert.isTrue("Ja� Kowalski" == name);
        }

		public void applyUpdatesTest()
        {
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1);

			ClientChatSystemTest clientChatSystem = new ClientChatSystemTest();
			clientChatSystem.applyUpdates(chatSystem.getUpdatesOfUser("Kasia �d�b�o"));
			bool conversationPresent = false;
			foreach (var conversation in clientChatSystem.getConversationsOfUser("Kasia�d�b�o"))
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
			Assert.isFalse(chatSystem.logInTest("Kasia �d�b�o");
			chatSystem.addNewUser("Kasia �d�b�o");
			Assert.isTrue(chatSystem.logInTest("Kasia �d�b�o");
			Assert.isTrue(chatSystem.getUserName() == "Kasia �d�b�o");
        }
	}

	[TestClass]
	public class UserTest
	{
		public void getNameTest()
        {
			string name = "Ja� Kowalski";
			User user1 = new User(name);
			Assert.isTrue(name == user1.getName());
        }

		public void getConversationsTest()
        {
			ChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			User user3 = chatSystem.addUser("Claus Somersby");
			User user3 = chatSystem.addUser("Hania Kot");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user2, user3);
			
			bool hasConversation1 = false;
			bool hasConversation2 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.getConversations())
            {
				if (conversation == savedConversation1)
					hasConversation1 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation1);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user2.getConversations())
            {
				if (conversation == savedConversation1)
					hasConversation1 = true;
				else if (conversation == savedConversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation1);
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user3.getConversations())
            {
				if (conversation == savedConversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user4.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
        }

		public void matchWithConversationTest()
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1); //dopuszczamy mo�liwo�� stworzenia pustej konwersacji do test�w
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			User user1 = new User("Pan A");
			user user2 = new user("Pani B");
			
			bool hasConversation1 = false;
			bool hasConversation2 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
			hasWrongConversation = false;
			foreach (var conversation in user2.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
			hasWrongConversation = false;
			bool hasUser1 = false;
			bool hasUser2 = false;
			bool hasWrongUser = false;
			foreach (var user in conversation1.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation2.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;

			bool methodResult;
			methodResult = user1.matchWithConversation(conversation1);
			Assert.isTrue(methodResult);
			methodResult = user1.matchWithConversation(conversation2);
			Assert.isTrue(methodResult);
			methodResult = user2.matchWithConversation(conversation1);
			Assert.isTrue(methodResult);		

			foreach (var conversation in user1.getConversations())
            {
				if (conversation == conversation1) 
					hasConversation1 = true;
				else if (conversation == conversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation1);
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user2.getConversations())
            {
				if (conversation == conversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation2);
			Assert.isFalse(hasWrongConversation);

			hasUser1 = false;
			hasUser2 = false;
			hasWrongUser = false;
			foreach (var user in conversation1.getUsers())
            {
				if (user == user1)
					hasUser1 = true;
				else if (user == user2)
					hasUser2 = true;
				else
					hasWrongUser = true;
            }
			Assert.isTrue(hasUser1);
			Assert.isTrue(hasUser2);
			Assert.isFalse(hasWrongUser);
			hasUser1 = false;
			hasUser2 = false;
			hasWrongUser = false;
			foreach (var user in conversation2.getUsers())
            {
				if (user == user1)
					hasUser1 = true;
				else
					hasWrongUser = true;
            }
			Assert.isTrue(hasUser1);
			Assert.isFalse(hasWrongUser);

			methodResult = user2.matchWithConversation(conversation1);
			Assert.isFalse(methodResult);
        }

		public void unmatchWithConversationTest()
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			User user1 = new User("Pan A");

			bool hasConversation1 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
			hasWrongConversation = false;
			bool hasUser1 = false;
			bool hasWrongUser = false;
			foreach (var user in conversation1.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;

			user1.matchWithConversation(conversation1);

			foreach (var conversation in user1.getConversations())
            {
				if (conversation == conversation1) 
					hasConversation1 = true;
				else
					hasWrongConversation = true;
            }
			Assert.isTrue(hasConversation1);
			Assert.isFalse(hasWrongConversation);
			hasConversation1 = false;
			hasWrongConversation = false;

			hasUser1 = false;
			hasWrongUser = false;
			foreach (var user in conversation1.getUsers())
            {
				if (user == user1)
					hasUser1 = true;
				else
					hasWrongUser = true;
            }
			Assert.isTrue(hasUser1);
			Assert.isFalse(hasWrongUser);
			hasUser1 = false;
			hasWrongUser = false;

			bool methodResult;
			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.isTrue(methodResult);			

			foreach (var conversation in user1.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
			hasWrongConversation = false;
			foreach (var user in conversation1.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;
			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.isFalse(methodResult);	
        }
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
