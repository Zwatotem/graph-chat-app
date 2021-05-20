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
			User user1 = new User("Jaœ Kowalski");
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.ID == 1);
		}

		[TestMethod]
		public void getTimeTest()
		{
			User user1 = new User("Jaœ Kowalski");
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
			User user1 = new User("Pszczó³ka Maja");
			User user2 = new User("Stary Trzmiel");
			conversation1.matchWithUser(user1);
			conversation2.matchWithUser(user1);
			conversation1.matchWithUser(user2);
			conversation2.matchWithUser(user2);
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			DateTime datetime2 = datetime + TimeSpan.FromSeconds(4);
			IMessageContent messageContent2 = new TextContent("Ano spoko");
			Message message1 = conversation1.addMessage(user1, -1, messageContent1, datetime);
			Message message2 = conversation1.addMessage(user2, 1, messageContent2, datetime2);

			Message recreatedMessage1 = conversation2.addMessage(message1.serialize(new ConcreteSerializer()), new ConcreteDeserializer());
			Message recreatedMessage2 = conversation2.addMessage(message2.serialize(new ConcreteSerializer()), new ConcreteDeserializer());

			Assert.AreEqual(recreatedMessage1.ID, message1.ID); // There is no way serialization keeps references
			Assert.AreEqual(recreatedMessage1.Author.getName(), message1.Author.getName());
			Assert.IsNull(recreatedMessage1.Parent);
			Assert.AreEqual(recreatedMessage1.Content.getData(), message1.Content.getData());
			Assert.AreEqual(recreatedMessage1.SentTime, message1.SentTime);

			Assert.AreEqual(recreatedMessage2.ID, message2.ID);
			Assert.AreEqual(recreatedMessage2.Author.getName(), message2.Author.getName());
			Assert.AreEqual(recreatedMessage2.Parent, recreatedMessage1);
			Assert.AreEqual(recreatedMessage2.Content.getData(), message2.Content.getData());
			Assert.AreEqual(recreatedMessage2.SentTime, message2.SentTime);
		}

		[TestMethod]
		public void getContentTest()
		{
			User user1 = new User("Jaœ Kowalski");
			DateTime datetime = DateTime.Now;
			IMessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.Content == messageContent1);
		}
	}
}