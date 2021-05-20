
namespace ChatModel
{
	public interface IMessageContent //interface representing the content of a message (such as it's text)
	{
		/// <summary>
		/// Returns the content of a message.
		/// </summary>
		/// <returns>An object of type depending on the concrete implementation.</returns>
		object getData();

		/// <summary>
		/// Serializes the object.
		/// </summary>
		/// <returns>A byte array containing the serialized object.</returns>
		byte[] serialize();
	}
}
