using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleNewUserStrategy : IHandleStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, ClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add new user");
            string proposedName = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("DEBUG: trying to add new user");
            User newUser = null;
            lock (chatServer)
            {
                newUser = chatSystem.addNewUser(proposedName);
            }
            byte[] reply = new byte[1];
            reply[0] = (newUser == null) ? (byte)0 : (byte)1;
            handlerThread.sendMessage(1, reply);
        }
    }
}
