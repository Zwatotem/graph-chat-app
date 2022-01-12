using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
	[TestClass]
	public class UserTest
	{
		[TestMethod]
		public void getNameTest()
		{
			string name = "Jaś Kowalski";
			IUser user1 = new User(name);
			Assert.IsTrue(name == user1.Name);
		}

		[TestMethod]
		public void getConversationsTest()
		{
			IChatSystem chatSystem = new ServerChatSystem();
			IUser user1 = chatSystem.AddNewUser("Jaś Kowalski");
			IUser user2 = chatSystem.AddNewUser("Kasia Źdźbło");
			IUser user3 = chatSystem.AddNewUser("Claus Somersby");
			IUser user4 = chatSystem.AddNewUser("Hania Kot");
			Conversation savedConversation1 = chatSystem.AddConversation("Konfa 1", user1, user2);
			Conversation savedConversation2 = chatSystem.AddConversation("Konfa 2", user2, user3);

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
			Conversation conversation1 = new Conversation("Konfa 1", 1); //dopuszczamy możliwość stworzenia samodzielnej konwersacji do testów
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
			methodResult = user1.MatchWithConversation(conversation1);
			Assert.IsTrue(methodResult);
			methodResult = user1.MatchWithConversation(conversation2);
			Assert.IsTrue(methodResult);
			methodResult = user2.MatchWithConversation(conversation1);
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

			methodResult = user2.MatchWithConversation(conversation1);
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

			user1.MatchWithConversation(conversation1);

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
			methodResult = user1.UnmatchWithConversation(conversation1);
			Assert.IsTrue(methodResult);

			foreach (var conversation in user1.Conversations)
			{
				hasWrongConversation = true;
			}
			Assert.IsFalse(hasWrongConversation);
			hasWrongConversation = false;

			methodResult = user1.UnmatchWithConversation(conversation1);
			Assert.IsFalse(methodResult);
		}
	}
}