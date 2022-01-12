using ChatModel;
using ChatModel.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace chatAppTest
{
	[TestClass]
	public class MessageTest
	{
		[TestMethod]
		public void getIdTest()
		{
			IUser user1 = new User("Jaś Kowalski");
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.ID == 1);
		}

		[TestMethod]
		public void getTimeTest()
		{
			IUser user1 = new User("Jaś Kowalski");
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.SentTime == datetime);
		}

		[TestMethod]
		public void serializeTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			IUser user1 = new User("Pszczółka Maja");
			IUser user2 = new User("Stary Trzmiel");
			conversation1.MatchWithUser(user1);
			conversation2.MatchWithUser(user1);
			conversation1.MatchWithUser(user2);
			conversation2.MatchWithUser(user2);
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			DateTime datetime2 = datetime + TimeSpan.FromSeconds(4);
			IMessageContent messageContent2 = new TextContent("Ano spoko");
			Message message1 = conversation1.AddMessage(user1, -1, messageContent1, datetime);
			Message message2 = conversation1.AddMessage(user2, 1, messageContent2, datetime2);

			Message recreatedMessage1 = conversation2.AddMessage(message1.Serialize(new ConcreteSerializer()), new ConcreteDeserializer());
			Message recreatedMessage2 = conversation2.AddMessage(message2.Serialize(new ConcreteSerializer()), new ConcreteDeserializer());

			Assert.AreEqual(recreatedMessage1.ID, message1.ID); // There is no way serialization keeps references
			Assert.AreEqual(recreatedMessage1.Author.Name, message1.Author.Name);
			Assert.IsNull(recreatedMessage1.Parent);
			Assert.AreEqual(recreatedMessage1.Content.getData(), message1.Content.getData());
			Assert.AreEqual(recreatedMessage1.SentTime, message1.SentTime);

			Assert.AreEqual(recreatedMessage2.ID, message2.ID);
			Assert.AreEqual(recreatedMessage2.Author.Name, message2.Author.Name);
			Assert.AreEqual(recreatedMessage2.Parent, recreatedMessage1);
			Assert.AreEqual(recreatedMessage2.Content.getData(), message2.Content.getData());
			Assert.AreEqual(recreatedMessage2.SentTime, message2.SentTime);
		}

		[TestMethod]
		public void getContentTest()
		{
			IUser user1 = new User("Jaś Kowalski");
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.Content == messageContent1);
		}
	}
}