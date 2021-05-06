
namespace ChatModel
{
	public interface MessageContent //interface representing the content of a message (such as it's text)
	{
		object getData(); //returns the content of a message

		object serialize(); //serializes the concrete implementation
	}
}
