using System;
using System.Collections.Generic;

namespace ChatModel
{
	public class ClientChatSystem : ChatSystem //concrete class derived from abstract ChatSystem. It representsthe chat system on the client side.									   //
	{
		private string userName; //user name of the user that is usign the client app and has already logged in

		public ClientChatSystem() : base() //no-arg constructor calling ChatSystem constructor
        {
			this.userName = null; //indicates that there is no logged in user at the start
        }

		public bool logIn(string login) //if there is no logged in user sets userName to parameter (if such user exists in the system)
		//and returns true, else returns false.
		{
			if (userName != null) //if someone is already logged in, returns false
            {
				return false;
            }
			if (users.Find(u => u.Name == login) != null) //if user with user name passed as parameter exists
			{
				userName = login;
				return true;
			}
			else
            {
				return false;
            }
		}

		public string getUserName() //returns the user name of the user currently logged in or null if there is no such user.
		{
			return userName;
		}

		public void applyUpdates(object p)
		{
			throw new NotImplementedException();
		}
	}
}