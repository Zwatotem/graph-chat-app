using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleNewUserStrategy : IHandleStrategy
    {
        /// <summary>
        /// Class handling request to create new user.
        /// </summary>
        public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add new user");
            //decoding request - all bytes are proposed user name
            string proposedName = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("DEBUG: trying to add new user");
            IUser newUser = null;
            lock (allHandlers)
            {
                newUser = chatSystem.addNewUser(proposedName);
            }
            byte[] reply = new byte[1];
            reply[0] = (newUser == null) ? (byte)0 : (byte)1; //if user was not created (eg. user name taken) indicate failure 
            handlerThread.sendMessage(1, reply);
        }
    }
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/