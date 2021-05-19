using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleNewUserStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add new user");
            string proposedName = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("DEBUG: trying to add new user");
            User newUser = null;
            lock (allHandlers)
            {
                newUser = chatSystem.addNewUser(proposedName);
            }
            byte[] reply = new byte[1];
            reply[0] = (newUser == null) ? (byte)0 : (byte)1;
            handlerThread.sendMessage(1, reply);
        }
    }
}
