using System;
using System.Collections.Generic;
using System.IO;

namespace ChatModel
{
	public abstract class ChatSystem //abstract class implementing shared properties of client and server chat systems
	{
		protected Dictionary<int, Conversation> conversations; //dictionary of all conversations in the chat system, indexed by their unique id
		protected List<User> users; //list of all users in the chat system, each has an unique user name
		protected int smallestFreeId; //smallest unique id available to be assigned to a new conversation
		protected Stack<int> freedIds; //stack of conversation ids smaller than current smallest available that were freed be deleting conversations

		public ChatSystem() //simple constructor, initializing collections as empty and setting first available conversation id to 1
		{
			this.conversations = new Dictionary<int, Conversation>();
			this.users = new List<User>();
			this.smallestFreeId = 1;
			this.freedIds = new Stack<int>();
		}

		public User addNewUser(string newUserName) //method to create a new user in the chat system, with an unique name passed as parameter.
												   //if successful, returns a reference to created user object. If there's already present a user with passed user name, returns null.
		{
			if (users.Exists(u => u.Name == newUserName))
			{ //checking if the proposed user name would be unique
				return null;
			}
			else
			{
				User newUser = new User(newUserName); //creating and adding new user to the list
				users.Add(newUser);
				return newUser;
			}
		}

		public User getUser(string userName) //method to return an existing user with a name passed as parameter. If successful, 
											 //returns object reference, if there's no such user in the chat system, return null.
		{
			return users.Find(u => u.Name == userName); //returns first found user with specific name (there's at most one as names are unique)
		}

		public Conversation getConversation(int id) //method to return a conversation with an id passed as parameter. If successful, 
													//returns object reference, if there's no such conversation in the chat system, return null.
		{
			if (conversations.ContainsKey(id)) //if conversation with a given id is present 
			{
				return conversations[id]; //returns a reference to it
			}
			else
			{
				return null; //if there is no such conversation, returns null
			}
		}

		public Conversation addConversation(string conversationName, params string[] ownersNames) //method to create new conversation with a name
																								  //passed as first parameter and list of user names of users that are to participate as second parameter.
																								  //returns reference to created conversation or null, if the conversation cannot be created (eg. not all users exist in the system)
		{
			User[] owners = new User[ownersNames.Length]; //creates an array to be filled with references to the new conversation's users
			int index = 0; //index of first free position in the array
			foreach (var userName in ownersNames) //finding all users in a loop
			{
				User userReference = users.Find(u => u.Name == userName); //finds user with a specific name
				if (userReference == null) //if there's no such user conversation cannot be created
				{
					return null;
				}
				else
				{
					owners[index++] = userReference; //if user found stores the reference in the array
				}
			}
			return addConversation(conversationName, owners); //as the users in owners array are present in the system, second version of
															  //the method can be called. Then it's returned value (a reference to created conversation) can be returned.
		}

		public Conversation addConversation(string conversationName, params User[] owners) //method to create new conversation with a name
																						   //passed as first parameter and list of references to users (that are expected to be present in the system!) as second parameter.
																						   //returns created conversation
		{
			int newId; //variable to store the id to be assigned to created conversation
			if (freedIds.Count > 0) //if there are any freed ids on the stack, one of the is going to be reused
			{
				newId = freedIds.Pop();
			}
			else
			{
				newId = smallestFreeId++; //else we take current smallest available id and set smallestFreeId to next integer
			}
			Conversation newConversation = new Conversation(conversationName, newId); //new conversation is created
			conversations.Add(newId, newConversation); //and added to collection, indexed with it's id
			foreach (var owner in owners) //each user passed as parameter:
			{
				newConversation.matchWithUser(owner); //has to assigned to newly created conversation
				owner.matchWithConversation(newConversation); //and the conversation has to be assigned to them
			}
			return newConversation; //returns created conversation
		}

		public Conversation addConversation(Stream stream) //creates a copy of a conversation passed as paramter in serialized form (?)
		{
			throw new NotImplementedException();
		}

		public bool addUserToConversation(string userName, int id) //method to add a user with a given user name to a conversation with given id
																   //returns true if operation successful, false if there is no such user or conversation or if the user is already assigned to it.
		{
			User userToAdd = getUser(userName); //getting reference to a user in the system with given user name
			if (userToAdd == null)
			{
				return false; //if there is no such user, indicate failure of the operation
			}
			Conversation conversationToAdd = getConversation(id); //getting reference to a conversation in the system with given id
			if (conversationToAdd == null)
			{
				return false; //if there is no such conversation, indicate failure of the operation
			}
			userToAdd.matchWithConversation(conversationToAdd); //assigning the conversation to the user
			return conversationToAdd.matchWithUser(userToAdd); //assigning the user to the conversation. If they are already assigned
															   //a false value is returned
		}

		public bool leaveConversation(string userName, int id) //method that removes the user with a name passed as first parameter from
															   //a conversation with an id passed as second parameter. If there would be no users left in it, the conversation gets deleted.
															   //return true if operation successful, false if unsuccessful (eg. there is no such user or they are not assigned to the conversation)
		{
			User userToRemove = getUser(userName); //getting reference to a user in the system with given user name
			if (userToRemove == null)
			{
				return false; //if there is no such user, indicate failure of the operation
			}
			Conversation conversation = getConversation(id); //getting reference to a conversation in the system with given id
			if (conversation == null)
			{
				return false; //if there is no such conversation, indicate failure of the operation
			}
			userToRemove.unmatchWithConversation(conversation); //removes the conversation from user
			bool result = conversation.unmatchWithUser(userToRemove); //and user from conversation
			if (!result) //if the user was not assigned to the conversation in the first place
			{
				return false; //indicate failure of the operation
			}
			else
			{
				if (conversation.getUsers().Count == 0) //if there would be no users in the conversation left
				{
					conversations.Remove(id); //deletes the conversation
					if (id == smallestFreeId - 1) //if the id of deleted conversation was only one smaller than smallestFreeId
					{
						smallestFreeId--; //decrement the variable
					}
					else
					{
						freedIds.Push(id); //push the id to the stack for reuse
					}
				}
				return true; //indicate success of the operation
			}
		}


		public Message sendMessage(int id, string userName, int messageId, MessageContent messageContent, DateTime sentTime) //method to send a message
																															 //to conversation with a given id, from a user with a given userName, replying to a message with a given id, with given content
																															 //and time of being sent. If successful returns reference to created message, else returns null.
		{
			Conversation conversation = getConversation(id); //getting reference to a conversation in the system with given id
			if (conversation == null)
			{
				return null; //if there is no such conversation, indicate failure of the operation
			}
			User author = conversation.getUsers().Find(u => u.Name == userName); //getting reference to a user in the conversation with given name
			if (author == null)
			{
				return null; //if there is no such user, indicate failure of the operation
			}
			return conversation.addMessage(author, messageId, messageContent, sentTime); //calls the conversation method responsible for
																						 //creating a message. Returns it's returned value. True if operation successful, else false.
		}
	}
}