using System.Collections.Generic;
using System.IO;

namespace ChatModel
{
	public interface IConversation
	{
		string Name { get; set; }
		int ID { get; }
		int getId();
		List<User> getUsers();
		string getName();
		ICollection<Message> getMessages();
		Message getMessage(int id);
		Message addMessageUnsafe(Message m);
		Stream serialize();
	}
}