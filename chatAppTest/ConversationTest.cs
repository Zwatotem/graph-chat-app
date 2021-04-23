using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class ConversationTest
	{
		[TestMethod] //czy taka anotacja powinna byÊ przy wszystkich?
		public void getNameTest()
		{
			string name = "Konwersacja 1";
			Conversation conversation1 = new Conversation(name, 1);
			Assert.IsTrue(name == conversation1.getName());
		}

		public void addMessageTest()
		{
			User user1 = new User("Roch Lancel");
			User user2 = new User("Harry Potter");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.IsNull(addedMessage);
			conversation1.matchWithUser(user1);
			Message addedMessage = conversation1.addMessage(user1, 2, msgContent1, datetime, 1);
			Assert.IsNull(addedMessage);
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.IsNotNull(addedMessage);
			Assert.IsTrue(user1 == addedMessage.getUser());
			Assert.IsNull(addedMessage.getParent());
			Assert.IsTrue(msgContent1 == addedMessage.getContent());
			Assert.IsTrue(datetime == addedMessage.getTime());
			Assert.IsTrue(1 == addedMessage.getId());

			conversation1.matchWithUser(user2);
			Content msgContent2 = new TextContent("Heeejoooo");
			DateTime datetime2 = DateTime.Now;
			Message nextMessage = conversation1.addMessage(user2, 1, msgContent2, datetime2, 2);
			Assert.IsNotNull(nextMessage);
			Assert.IsTrue(user2 == nextMessage.getUser());
			Assert.IsTrue(addedMessage == nextMessage.getParent());
			Assert.IsTrue(msgContent2 == nextMessage.getContent());
			Assert.IsTrue(datetime2 == nextMessage.getTime());
			Assert.IsTrue(2 == nextMessage.getId());

			bool hasMsg1 = false;
			bool hasMsg2 = false;
			bool hasWrongMessage = false;
			foreach (var msg in conversation1.getMessages())
			{
				if (msg == addedMessage)
					hasMsg1 = true;
				else if (msg = nextMessage)
					hasMsg2 = true;
				else
					hasWrongMessage = true;
			}
			Assert.IsTrue(hasMsg1);
			Assert.IsTrue(hasMsg2);
			Assert.IsFalse(hasWrongMessage);
		}

		public void getMessageTest()
		{
			User user1 = new User("Roch Lancel");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			conversation1.matchWithUser(user1);
			Assert.IsNull(conversation1.getMessage(1));
			Content msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime, 1);
			Assert.IsTrue(addedMessage == conversation1.getMessage(1));
			Assert.IsNull(conversation1.getMessage(2));
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
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation2.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			bool methodResult;
			methodResult = conversation1.matchWithUser(user1);
			Assert.IsTrue(methodResult);
			methodResult = conversation1.matchWithUser(user2);
			Assert.IsTrue(methodResult);
			methodResult = conversation2.matchWithUser(user1);
			Assert.IsTrue(methodResult);

			foreach (var user in conversation1.getUsers())
			{
				if (user == user1)
					hasUser1 = true;
				else if (user == user2)
					hasUser2 = true;
				else
					hasWrongUser = true;
			}
			Assert.IsTrue(hasUser1);
			Assert.IsTrue(hasUser2);
			Assert.IsFalse(hasWrongUser);
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
			Assert.IsTrue(hasUser1);
			Assert.IsFalse(hasWrongUser);

			methodResult = conversation2.matchWithUser(user1);
			Assert.IsFalse(methodResult);
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
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			conversation1.matchWithUser(user1);

			foreach (var user in conversation1.getUsers())
			{
				if (user == user1)
					hasUser1 = true;
				else
					hasWrongUser = true;
			}
			Assert.IsTrue(hasUser1);
			Assert.IsFalse(hasWrongUser);
			hasUser1 = false;
			hasWrongUser = false;

			bool methodResult;
			methodResult = conversation1.unmatchWithUser(user1);
			Assert.IsTrue(methodResult);

			foreach (var user in conversation1.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			methodResult = conversation1.unmatchWithUser(user1);
			Assert.IsFalse(methodResult);
		}

		public void getUsersTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			Conversation conversation3 = new Conversation("Konfa 3", 3);
			User user1 = new User("Pan A");
			User user2 = new User("Pani B");

			bool hasUser1 = false;
			bool hasUser2 = false;
			bool hasWrongUser = false;

			foreach (var user in conversation1.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation2.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation3.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
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
			Assert.IsTrue(hasUser1);
			Assert.IsTrue(hasUser2);
			Assert.IsFalse(hasWrongUser);
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
			Assert.IsTrue(hasUser1);
			Assert.IsTrue(hasUser2);
			Assert.IsFalse(hasWrongUser);
			hasUser1 = false;
			hasUser2 = false;
			hasWrongUser = false;
			foreach (var user in conversation3.getUsers())
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
		}

		public void serializeTest() //niekoniecznie tak ten test powinien wyglπdaÊ
		{
			ServerChatSystem serverChatSystem = new ServerChatSystem();
			User user1 = chatSystem.addUser("Jaú Kowalski");
			User user2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("Heeej");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.getId(), "Jaú Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.getId(), "Kasia èdüb≥o", sentMessage1.getId(), msgContent2, datetime);

			ClientChatSystem clientChatSystem = new ClientChatSystem();
			User userClient1 = chatSystem.addUser("Jaú Kowalski");
			User userClient2 = chatSystem.addUser("Kasia èdüb≥o");
			Conversation savedClientConversation = clientChatSystem.addConversation(savedConversation.serialize());

			bool msg1Present = false;
			bool msg2Present = false;
			bool wrongMsgPresent = false;
			foreach (var message in savedClientConversation.getMessages())
			{
				if (message.getId() == sentMessage1.getId())
				{
					msg1Present = true;
					Assert.IsTrue(message.getUser().getName() == sentMessage1.getUser().getName());
					Assert.IsNull(message.getParent());
					Assert.IsTrue(message.getContent().getData() == sentMessage1.getContent().getData());
					Assert.IsTrue(message.getTime().Equals(sentMessage1.getTime()));
				}
				else if (message.getId() == sentMessage2.getId())
				{
					msg2Present = true;
					Assert.IsTrue(message.getUser().getName() == sentMessage2.getUser().getName());
					Assert.IsTrue(message.getParent().getId() == sentMessage1.getId());
					Assert.IsTrue(message.getContent().getData() == sentMessage2.getContent().getData());
					Assert.IsTrue(message.getTime().Equals(sentMessage2.getTime()));
				}
				else
					wrongMsgPresent = true;
			}
			Assert.IsTrue(msg1Present);
			Assert.IsTrue(msg2Present);
			Assert.IsFalse(wrongMsgPresent);
		}

		public void getUpdatesTest() //czy tak?
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			User user1 = new User("Mr. X");
			User user2 = new User("Ms. Y");
			Content msgContent1 = new TextContent("Heeejoooo");
			Content msgContent2 = new TextContent("No czeúÊ");
			Content msgContent3 = new TextContent("Co tam s≥ychaÊ?");
			DateTime datetime = DateTime.Now;
			DateTime datetime2 = datetime + TimeSpan.FromSeconds(5);
			DateTime datetime3 = datetime + TimeSpan.FromSeconds(19);
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
					Assert.IsTrue(message.getUser().getName() == sentMessage2.getUser().getName());
					Assert.IsTrue(message.getParent().getId() == 1);
					Assert.IsTrue(message.getContent().getData() == sentMessage2.getContent().getData());
					Assert.IsTrue(message.getTime() == sentMessage2.getTime());
				}
				else if (message.getId() == sentMessage2.getId())
				{
					hasMessage3 = true;
					Assert.IsTrue(message.getUser().getName() == sentMessage3.getUser().getName());
					Assert.IsTrue(message.getParent().getId() == 2);
					Assert.IsTrue(message.getContent().getData() == sentMessage3.getContent().getData());
					Assert.IsTrue(message.getTime() == sentMessage3.getTime());
				}
				else
					hasWrongMessage = true;
			}
			Assert.IsTrue(hasMessage2);
			Assert.IsTrue(hasMessage3);
			Assert.IsFalse(hasWrongMessage);
		}
	}
}