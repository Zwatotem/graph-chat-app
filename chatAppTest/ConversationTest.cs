using ChatModel;
using ChatModel.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace chatAppTest
{
	[TestClass]
	public class ConversationTest
	{
		[TestMethod]
		public void getNameTest()
		{
			string name = "Konwersacja 1";
			Conversation conversation1 = new Conversation(name, 1);
			Assert.IsTrue(name == conversation1.Name);
		}

		[TestMethod]
		public void addMessageTest()
		{
			User user1 = new User("Roch Lancel");
			User user2 = new User("Harry Potter");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message addedMessage1 = conversation1.addMessage(user1, -1, msgContent1, datetime);
			Assert.IsNull(addedMessage1);
			conversation1.matchWithUser(user1);
			Message addedMessage2 = conversation1.addMessage(user1, 2, msgContent1, datetime);
			Assert.IsNull(addedMessage2);
			Message addedMessage3 = conversation1.addMessage(user1, -1, msgContent1, datetime);
			Assert.IsNotNull(addedMessage3);
			Assert.IsTrue(user1 == addedMessage3.Author);
			Assert.IsNull(addedMessage3.Parent);
			Assert.IsTrue(msgContent1 == addedMessage3.Content);
			Assert.IsTrue(datetime == addedMessage3.SentTime);
			Assert.IsTrue(1 == addedMessage3.ID);

			conversation1.matchWithUser(user2);
			IMessageContent msgContent2 = new TextContent("Heeejoooo");
			DateTime datetime2 = DateTime.Now;
			Message nextMessage = conversation1.addMessage(user2, 1, msgContent2, datetime2);
			Assert.IsNotNull(nextMessage);
			Assert.IsTrue(user2 == nextMessage.Author);
			Assert.IsTrue(addedMessage3 == nextMessage.Parent);
			Assert.IsTrue(msgContent2 == nextMessage.Content);
			Assert.IsTrue(datetime2 == nextMessage.SentTime);
			Assert.IsTrue(2 == nextMessage.ID);

			bool hasMsg1 = false;
			bool hasMsg2 = false;
			bool hasWrongMessage = false;
			foreach (var msg in conversation1.Messages)
			{
				if (msg == addedMessage3)
					hasMsg1 = true;
				else if (msg == nextMessage)
					hasMsg2 = true;
				else
					hasWrongMessage = true;
			}
			Assert.IsTrue(hasMsg1);
			Assert.IsTrue(hasMsg2);
			Assert.IsFalse(hasWrongMessage);
		}

		[TestMethod]
		public void getMessageTest()
		{
			User user1 = new User("Roch Lancel");
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			conversation1.matchWithUser(user1);
			Assert.IsNull(conversation1.getMessage(1));
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			DateTime datetime = DateTime.Now;
			Message addedMessage = conversation1.addMessage(user1, -1, msgContent1, datetime);
			Assert.IsTrue(addedMessage == conversation1.getMessage(1));
			Assert.IsNull(conversation1.getMessage(2));
		}

		[TestMethod]
		public void matchWithUserTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			User user1 = new User("Pan A");
			User user2 = new User("Pani B");

			bool hasUser1 = false;
			bool hasUser2 = false;
			bool hasWrongUser = false;
			foreach (var user in conversation1.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation2.Users)
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

			foreach (var user in conversation1.Users)
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
			foreach (var user in conversation2.Users)
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

		[TestMethod]
		public void unmatchWithUserTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			User user1 = new User("Pan A");

			bool hasUser1 = false;
			bool hasWrongUser = false;
			foreach (var user in conversation1.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			conversation1.matchWithUser(user1);

			foreach (var user in conversation1.Users)
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

			foreach (var user in conversation1.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			methodResult = conversation1.unmatchWithUser(user1);
			Assert.IsFalse(methodResult);
		}

		[TestMethod]
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

			foreach (var user in conversation1.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation2.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;
			foreach (var user in conversation3.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
			hasWrongUser = false;

			conversation1.matchWithUser(user1);
			conversation1.matchWithUser(user2);
			conversation2.matchWithUser(user1);

			foreach (var user in conversation1.Users)
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
			foreach (var user in conversation2.Users)
			{
				if (user == user1)
					hasUser1 = true;
				else
					hasWrongUser = true;
			}
			Assert.IsTrue(hasUser1);
			Assert.IsFalse(hasUser2);
			Assert.IsFalse(hasWrongUser);
			hasUser1 = false;
			hasUser2 = false;
			hasWrongUser = false;
			foreach (var user in conversation3.Users)
			{
				hasWrongUser = true;
			}
			Assert.IsFalse(hasWrongUser);
		}

		[TestMethod]
		public void serializeTest()
		{
			ServerChatSystem chatSystem = new ServerChatSystem();
			User user1 = chatSystem.addNewUser("Jaú Kowalski");
			User user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedConversation = chatSystem.addConversation("Konfa 1", user1, user2);
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			IMessageContent msgContent2 = new TextContent("Heeej");
			DateTime datetime = DateTime.Now;
			Message sentMessage1 = chatSystem.sendMessage(savedConversation.ID, "Jaú Kowalski", -1, msgContent1, datetime);
			Message sentMessage2 = chatSystem.sendMessage(savedConversation.ID, "Kasia èdüb≥o", sentMessage1.ID, msgContent2, datetime);

			ClientChatSystem clientChatSystem = new ClientChatSystem();
			User userClient1 = clientChatSystem.addNewUser("Jaú Kowalski");
			User userClient2 = clientChatSystem.addNewUser("Kasia èdüb≥o");
			Conversation savedClientConversation = clientChatSystem.addConversation(savedConversation.serialize(new ConcreteSerializer()), new ConcreteDeserializer());

			bool msg1Present = false;
			bool msg2Present = false;
			bool wrongMsgPresent = false;
			foreach (var message in savedClientConversation.Messages)
			{
				if (message.ID == sentMessage1.ID)
				{
					msg1Present = true;
					Assert.AreEqual(message.Author.Name, userClient1.Name); // There is no way serialization keeps references
					Assert.AreSame(message.Author, savedClientConversation.Users.ToArray()[0]);
					Assert.IsNull(message.Parent);
					Assert.AreEqual(message.Content.getData(), sentMessage1.Content.getData());
					Assert.AreEqual(message.SentTime, sentMessage1.SentTime);
				}
				else if (message.ID == sentMessage2.ID)
				{
					msg2Present = true;
					Assert.AreEqual(message.Author.Name, userClient2.Name);
					Assert.IsNotNull(message.Parent.ID); //to be deleted when test passes
					Assert.AreEqual(message.TargetId, sentMessage1.ID); //to be deleted but this passes
					Assert.AreEqual(message.Parent.ID, sentMessage1.ID); //while this causes the test to fail
					Assert.AreEqual(message.Content.getData(), sentMessage2.Content.getData());
					Assert.AreEqual(message.SentTime, sentMessage2.SentTime);
				}
				else
					wrongMsgPresent = true;
			}
			Assert.IsTrue(msg1Present);
			Assert.IsTrue(msg2Present);
			Assert.IsFalse(wrongMsgPresent);
		}

		[TestMethod]
		public void getUpdatesTest()
		{
			// Conversation
			Conversation conversation1 = new Conversation("Konfa 1", 1);

			// Users
			User user1 = new User("Mr. X");
			User user2 = new User("Ms. Y");
			// Matching
			conversation1.matchWithUser(user1);
			conversation1.matchWithUser(user2);
			user1.matchWithConversation(conversation1);
			user2.matchWithConversation(conversation1);

			// Messages
			//  Content
			IMessageContent msgContent1 = new TextContent("Heeejoooo");
			IMessageContent msgContent2 = new TextContent("No czeúÊ");
			IMessageContent msgContent3 = new TextContent("Co tam s≥ychaÊ?");
			//  Timestamps
			DateTime datetime = DateTime.Now;
			DateTime datetime2 = datetime + TimeSpan.FromSeconds(5);
			DateTime datetime3 = datetime + TimeSpan.FromSeconds(19);
			//  Sending
			Message sentMessage1 = conversation1.addMessage(user1, -1, msgContent1, datetime);
			Message sentMessage2 = conversation1.addMessage(user2, 1, msgContent2, datetime2);
			Message sentMessage3 = conversation1.addMessage(user1, 2, msgContent3, datetime3);

			// Checks
			bool hasMessage2 = false;
			bool hasMessage3 = false;
			bool hasWrongMessage = false;
			foreach (var message in conversation1.getUpdates(1).Messages)
			{
				if (message.ID == sentMessage2.ID)
				{
					hasMessage2 = true;
					Assert.IsTrue(message.Author.getName() == sentMessage2.Author.getName());
					Assert.IsTrue(message.TargetId == 1);
					Assert.IsTrue(message.Content.getData() == sentMessage2.Content.getData());
					Assert.IsTrue(message.SentTime == sentMessage2.SentTime);
				}
				else if (message.ID == sentMessage3.ID)
				{
					hasMessage3 = true;
					Assert.IsTrue(message.Author.getName() == sentMessage3.Author.getName());
					Assert.IsTrue(message.Parent.ID == 2);
					Assert.IsTrue(message.Content.getData() == sentMessage3.Content.getData());
					Assert.IsTrue(message.SentTime == sentMessage3.SentTime);
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