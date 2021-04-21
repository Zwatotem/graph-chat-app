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
			DateTime datetime = new DateTime(now);
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1, datetime);
			Assert.IsNotNull(sentMessage1);
			Assert.IsNull(sentMessage1.getParent());
			Content msgContent2 = new TextContent("Cze��");
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia �d�b�o", sentMessage1.getId(), msgContent2, datetime);
			Assert.IsNotNull(sentMessage2);
			Assert.IsTrue(sentMessage2.getParent() == sentMessage1); //test comment
		}
	}

	[TestClass]
	public class ServerChatSystemTest
	{
		public void getUpdatesToUserTest() //do przejrzenia czy na pewno tak to robimy
        {
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			User user3 = chatSystem.addUser("Roch Kowal");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user1, user3);
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("Heeej");
			DateTime datetime = new DateTime(now);
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation2.getId(), "Ja� Kowalski", -1, msgContent2, datetime);
			ICollection updates = chatSystem.getUpdatesOfUser("Kasia �d�b�o"); //czy na pewno tak przechowujemy updaty?
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
							Assert.isTrue(message.getUser().getName() == sentMessage1.getUser().getName());
							Assert.isNull(message.getParent());
							Assert.isTrue(message.getContent().getData() == sentMessage1.getContent().getData());
							Assert.isTrue(message.getTime() == sentMessage1.getTime());
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
			DateTime datetime = new DateTime(now);
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

			bool methodResult;
			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.isTrue(methodResult);			

			foreach (var conversation in user1.getConversations())
            {
				hasWrongConversation = true;
            }
			Assert.isFalse(hasWrongConversation);
			hasWrongConversation = false;

			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.isFalse(methodResult);	
        }
	}

	[TestClass]
	public class ConversationTest
	{
		[TestMethod] //czy taka anotacja powinna by� przy wszystkich?
		public void getNameTest() 
		{
			string name = "Konwersacja 1";
			ConversationTest conversation1 = new Conversation(name, 1);
			Assert.isTrue(name == conversation1.getName());
		}

		public void addMessageTest()
        {
			User user1 = new User("Roch Lancel");
			User user2 = new User("Harry Potter");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = new DateTime(now);
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.isNull(addedMessage);
			conversation1.matchWithUser(user1);
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.isNotNull(addedMessage);
			Assert.isTrue(user1 == addedMessage.getUser());
			Assert.isNull(addedMessage.getParent());
			Assert.isTrue(msgContent1 == addedMessage.getContent());
			Assert.isTrue(datetime == addedMessage.getTime());
			Assert.isTrue(1 == addedMessage.getId());

			conversation1.matchWithUser(user2);
			Content msgContent2 = new TextContent("Heeejoooo");
			DateTime datetime2 = new DateTime(now);
			Message nextMessage = conversation1.addMessage(user2, 1, msgContent2, datetime2, 2);
			Assert.isNotNull(nextMessage);
			Assert.isTrue(user2 == nextMessage.getUser());
			Assert.isTrue(addedMessage == nextMessage.getParent());
			Assert.isTrue(msgContent2 == nextMessage.getContent());
			Assert.isTrue(datetime2 == nextMessage.getTime());
			Assert.isTrue(2 == nextMessage.getId());
        }

		public void getMessageTest()
        {
			User user1 = new User("Roch Lancel");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			conversation1.matchWithUser(user1);
			Assert.isNull(conversation1.getMessage(1));
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = new DateTime(now);
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.isTrue(addedMessage == conversation1.getMessage(1));
			Assert.isNull(conversation1.getMessage(2));
        }

		public void matchWithUserTest()
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			User user1 = new User("Pan A");
			user user2 = new user("Pani B");
			
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
			methodResult = conversation1.matchWithUser(user1);
			Assert.isTrue(methodResult);
			methodResult = conversation1.matchWithUser(user2);
			Assert.isTrue(methodResult);
			methodResult = conversation2.matchWithUser(user1);
			Assert.isTrue(methodResult);		

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

			methodResult = conversation2.matchWithUser(user1);
			Assert.isFalse(methodResult);
        }

		public void unmatchWithUserTest()
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			User user1 = new User("Pan A");

			bool hasUser1 = false;
			bool hasWrongUser = false;
			foreach (var user in conversation1.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;

			conversation1.matchWithUser(user1);

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
			methodResult = conversation1.unmatchWithUser(user1);
			Assert.isTrue(methodResult);			

			foreach (var user in conversation1.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;

			methodResult = conversation1.unmatchWithUser(user1);
			Assert.isFalse(methodResult);	
        }

		public void getUsersTest()
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			Conversation conversation3 = new Conversation("Konfa 3", 3);
			User user1 = new User("Pan A");
			user user2 = new user("Pani B");

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
			foreach (var user in conversation3.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
			hasWrongUser = false;

			conversation1.matchWithUser(user1);
			conversation1.matchWithUser(user2);
			conversation2.matchWithUser(user1);

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
			Assert.isTrue(hasUser2);
			Assert.isFalse(hasWrongUser);
			hasUser1 = false;
			hasUser2 = false;
			hasWrongUser = false;
			foreach (var user in conversation3.getUsers())
            {
				hasWrongUser = true;
            }
			Assert.isFalse(hasWrongUser);
        }

		public void serializeTest() //niekoniecznie tak ten test powinien wygl�da�
        {
			ServerChatSystem serverChatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Ja� Kowalski");
			User user2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("Heeej");
			DateTime datetime = new DateTime(now);
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Ja� Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia �d�b�o", sentMessage1.getId(), msgContent2, datetime);

			ClientChatSystem clientChatSystem = new ClientChatSystem();
			User userClient1 = chatSystem.addUser("Ja� Kowalski");
			User userClient2 = chatSystem.addUser("Kasia �d�b�o");
			Conversation savedClientConversation = clientChatSystem.addConversation(savedConversation.serialize());

			bool msg1Present = false;
			bool msg2Present = false;
			bool wrongMsgPresent = false;
			foreach (var message in savedClientConversation.getMessages())
            {
				if (message.getId() == sentMessage1.getId())
                {
					msg1Present = true;
					Assert.isTrue(message.getUser().getName() == sentMessage1.getUser().getName());
					Assert.isNull(message.getParent());
					Assert.isTrue(message.getContent().getData() == sentMessage1.getContent().getData());
					Assert.isTrue(message.getTime().Equals(sentMessage1.getTime()));
                }
				else if (message.getId() == sentMessage2.getId())
                {
					msg2Present = true;
					Assert.isTrue(message.getUser().getName() == sentMessage2.getUser().getName());
					Assert.isTrue(message.getParent().getId() == sentMessage1.getId());
					Assert.isTrue(message.getContent().getData() == sentMessage2.getContent().getData());
					Assert.isTrue(message.getTime().Equals(sentMessage2.getTime()));
                }
				else
					wrongMsgPresent = true;
            }
			Assert.isTrue(msg1Present);
			Assert.isTrue(msg2Present);
			Assert.isFalse(wrongMsgPresent);
        }

		public void getUpdatesTest() //czy tak?
        {
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			User user1 = new User("Mr. X");
			User user2 = new User("Ms. Y");
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("No cze��");
			Content msgContent3 = new TextContent("Co tam s�ycha�?");
			DateTime datetime = new DateTime(now);
			DateTime datetime2 = datetime + 5;
			DateTime datetime3 = datetime + 19;
			Message sentMessage1 = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Message sentMessage2 = conversation1.addMessage(user2, 1, msgContent2, datetime2, 2);
			Message sentMessage3 = conversation1.addMessage(user1, 2, msgContent3, datetime3, 3);

			bool hasMessage2 = false;
			bool hasMessage3 = false;
			bool hasWrongMessage = false;
			foreach (var message in conversation1.getUpdates(1).getMessages())
            {
				if (message.getId() == sentMessage2.getId())
                {
					hasMessage2 = true;
					Assert.isTrue(message.getUser().getName() == sentMessage2.getUser().getName());
					Assert.isTrue(message.getParent().getId() == 1);
					Assert.isTrue(message.getContent().getData() == sentMessage2.getContent().getData());
					Assert.isTrue(message.getTime() == sentMessage2.getTime());
                }
				else if (message.getId() == sentMessage2.getId())
                {
					hasMessage3 = true;
					Assert.isTrue(message.getUser().getName() == sentMessage3.getUser().getName());
					Assert.isTrue(message.getParent().getId() == 2);
					Assert.isTrue(message.getContent().getData() == sentMessage3.getContent().getData());
					Assert.isTrue(message.getTime() == sentMessage3.getTime());
                }
				else
					hasWrongMessage = true;
            }
			Assert.isTrue(hasMessage2);
			Assert.isTrue(hasMessage3);
			Assert.isFalse(hasWrongMessage);
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
