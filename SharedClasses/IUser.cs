using System.Collections.Generic;

namespace ChatModel
{
	/// <summary>
	/// Interface representing a user of the chat system.
	/// </summary>
    public interface IUser
    {
		/// <summary>
		/// User name of the user.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// List of conversations in which the user takes part.
		/// </summary>
		List<Conversation> Conversations { get; }

		/// <summary>
		/// Adds a conversation to the list of conversations in which the user takes part.
		/// </summary>
		/// <param name="conversation">Conversation to match</param>
		/// <returns>True if successful, false otherwise.</returns>
		bool matchWithConversation(Conversation conversation);

		/// <summary>
		/// Removes a conversation from the list of all in which the user takes part.
		/// </summary>
		/// <param name="conversation1"></param>
		/// <returns>True if successful, false otherwise.</returns>
		bool unmatchWithConversation(Conversation conversation1);
	}
}

/*
Good example of compliance with SOLID:
1. Has only one responsibility - representing a user.
2. Encourages extension over modification - if you want a user to behave differently, just implement the interface anew.
4. Interface has only a few essential methods.
5. Allows for dependency inversion - referencing it, an abstract type, rather that a concrete class.
*/
