
namespace ChatModel
{
	/// <summary>
	/// Interface representing the content of a message.
	/// </summary>
	public interface IMessageContent
	{
		/// <summary>
		/// Returns the content of a message.
		/// </summary>
		/// <returns>An object of type dependant on the concrete implementation.</returns>
		object getData();

		/// <summary>
		/// Serializes the object.
		/// </summary>
		/// <returns>A byte array containing the serialized object.</returns>
		byte[] serialize();
	}
}

/*
This interface complies with SOLID principles:
1. It's only resposibility is storing and providing access to data (in normal or serialized form).
2. It encourages extension over modification, as one can just implement this interface anew rather than change code in existing implementations.
4. It has only the minimal number of methods - interface segregation.
5. It allows for dependency inversion - other code can reference this interface rather than concrete classes.
*/