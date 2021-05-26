using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class UserTest
	{
		[TestMethod]
		public void getNameTest()
		{
			string name = "Jaú Kowalski";
			IUser user1 = new User(name);
			Assert.IsTrue(name == user1.Name);
		}

		[TestMethod]
		public void getConversationsTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.addNewUser("Jaú Kowalski");
			IUser user2 = chatSystem.addNewUser("Kasia èdüb≥o");
			IUser user3 = chatSystem.addNewUser("Claus Somersby");
			IUser user4 = chatSystem.addNewUser("Hania Kot");
			Conversation savedConversation1 = chatSystem.addConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.addConversation("Konfa 2", user2, user3);

			bool hasConversation1 = false;
			bool hasConversation2 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.Conversations)
			{
				if (conversation == savedConversation1)
					hasConversation1 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user2.Conversations)
			{
				if (conversation == savedConversation1)
					hasConversation1 = true;
				else if (conversation == savedConversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsTrue(hasConversation2);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user3.Conversations)
			{
				if (conversation == savedConversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation2);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user4.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
		}

		[TestMethod]
		public void matchWithConversationTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1); //dopuszczamy moøliwoúÊ stworzenia samodzielnej konwersacji do testÛw
			Conversation conversation2 = new Conversation("Konfa 2", 2);
			IUser user1 = new User("Pan A");
			IUser user2 = new User("Pani B");

			bool hasConversation1 = false;
			bool hasConversation2 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
			hasWrongConversation = false;
			foreach (var conversation in user2.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
			hasWrongConversation = false;

			bool methodResult;
			methodResult = user1.matchWithConversation(conversation1);
			Assert.IsTrue(methodResult);
			methodResult = user1.matchWithConversation(conversation2);
			Assert.IsTrue(methodResult);
			methodResult = user2.matchWithConversation(conversation1);
			Assert.IsTrue(methodResult);

			foreach (var conversation in user1.Conversations)
			{
				if (conversation == conversation1)
					hasConversation1 = true;
				else if (conversation == conversation2)
					hasConversation2 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsTrue(hasConversation2);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasConversation2 = false;
			hasWrongConversation = false;
			foreach (var conversation in user2.Conversations)
			{
				if (conversation == conversation1)
					hasConversation1 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsFalse(hasWrongConversation);

			methodResult = user2.matchWithConversation(conversation1);
			Assert.IsFalse(methodResult);
		}

		[TestMethod]
		public void unmatchWithConversationTest()
		{
			Conversation conversation1 = new Conversation("Konfa 1", 1);
			IUser user1 = new User("Pan A");

			bool hasConversation1 = false;
			bool hasWrongConversation = false;
			foreach (var conversation in user1.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
			hasWrongConversation = false;

			user1.matchWithConversation(conversation1);

			foreach (var conversation in user1.Conversations)
			{
				if (conversation == conversation1)
					hasConversation1 = true;
				else
					hasWrongConversation = true;
			}
			Assert.IsTrue(hasConversation1);
			Assert.IsFalse(hasWrongConversation);
			hasConversation1 = false;
			hasWrongConversation = false;

			bool methodResult;
			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.IsTrue(methodResult);

			foreach (var conversation in user1.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
			hasWrongConversation = false;

			methodResult = user1.unmatchWithConversation(conversation1);
			Assert.IsFalse(methodResult);
		}
	}
}