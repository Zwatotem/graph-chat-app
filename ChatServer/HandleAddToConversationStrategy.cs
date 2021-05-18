using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    class HandleAddToConversationStrategy : IHandleRequestStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, HandlerThread handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add user to conversation");
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            string nameToAdd = Encoding.UTF8.GetString(messageBytes, 4, messageBytes.Length - 4);
            Console.WriteLine("DEBUG: trying to add user to conversation");
            byte[] reply = new byte[1];
            lock (chatServer)
            {
                if (chatSystem.addUserToConversation(nameToAdd, conversationId))
                {
                    reply[0] = 1;
                    byte[] msg = messageBytes;
                    Conversation conversation = chatSystem.getConversation(conversationId);
                    foreach (var handler in chatServer.Handlers.FindAll(h => conversation.getUsers().Exists(u => u.getName() == h.HandledUserName)))
                    {
                        if (handler.HandledUserName == nameToAdd)
                        {
                            byte[] update = conversation.serialize().ToArray();
                            handler.speak(5, update);
                        }
                        else
                        {
                            handler.speak(4, msg);
                        }
                    }
                }
                else
                {
                    reply[0] = 0;
                }
            }
            handlerThread.speak(1, reply);
        }
    }
}
