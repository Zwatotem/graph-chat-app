using System;
using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			MessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.getId() == 1);
		}

		[TestMethod]
		public void getTimeTest()
		{
			User user1 = new User("Jaœ Kowalski");
			DateTime datetime = DateTime.Now;
			MessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.getTime() == datetime);
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
			MessageContent messageContent1 = new TextContent("Siemka, co tam?");
			DateTime datetime2 = datetime + TimeSpan.FromSeconds(4);
			MessageContent messageContent2 = new TextContent("Ano spoko");
			Message message1 = conversation1.addMessage(user1, -1, messageContent1, datetime);
			Message message2 = conversation1.addMessage(user2, 1, messageContent2, datetime2);

			Message recreatedMessage1 = conversation2.addMessage(message1.serialize());
			Message recreatedMessage2 = conversation2.addMessage(message2.serialize());

			Assert.IsTrue(recreatedMessage1.getId() == message1.getId());
			Assert.IsTrue(recreatedMessage1.getUser().getName() == message1.getUser().getName());
			Assert.IsNull(recreatedMessage1.getParent());
			Assert.IsTrue(recreatedMessage1.getContent().getData() == message1.getContent().getData());
			Assert.IsTrue(recreatedMessage1.getTime() == message1.getTime());

			Assert.IsTrue(recreatedMessage2.getId() == message2.getId());
			Assert.IsTrue(recreatedMessage2.getUser().getName() == message2.getUser().getName());
			Assert.IsTrue(recreatedMessage2.getParent() == recreatedMessage1);
			Assert.IsTrue(recreatedMessage2.getContent().getData() == message2.getContent().getData());
			Assert.IsTrue(recreatedMessage2.getTime() == message2.getTime());
		}

		[TestMethod]
		public void getContentTest()
		{
			User user1 = new User("Jaœ Kowalski");
			DateTime datetime = DateTime.Now;
			MessageContent messageContent1 = new TextContent("Siemka, co tam?");
			Message message1 = new Message(user1, null, messageContent1, datetime, 1);
			Assert.IsTrue(message1.getContent() == messageContent1);
		}
	}
}