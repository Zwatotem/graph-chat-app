using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
    class HandleAddToConversationStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, IChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add user to conversation");
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            string nameToAdd = Encoding.UTF8.GetString(messageBytes, 4, messageBytes.Length - 4);
            Console.WriteLine("DEBUG: trying to add user to conversation");
            byte[] reply = new byte[1];
            lock (allHandlers)
            {
                if (chatSystem.addUserToConversation(nameToAdd, conversationId))
                {
                    reply[0] = 1;
                    byte[] msg = messageBytes;
                    Conversation conversation = chatSystem.getConversation(conversationId);
                    foreach (var handler in allHandlers.FindAll(h => conversation.Users.Exists(u => u.Name == h.HandledUserName)))
                    {
                        if (handler.HandledUserName == nameToAdd)
                        {
                            byte[] update = conversation.serialize(new ConcreteSerializer()).ToArray();
                            handler.sendMessage(5, update);
                        }
                        else
                        {
                            handler.sendMessage(4, msg);
                        }
                    }
                }
                else
                {
                    reply[0] = 0;
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}
