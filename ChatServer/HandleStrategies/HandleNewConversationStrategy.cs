using System;
using System.Collections.Generic;
using System.Text;
using ChatModel;
using ChatModel.Util;

namespace ChatServer.HandleStrategies
{
    class HandleNewConversationStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "add new conversation");
            List<string> namesOfParticipants = new List<string>();
            int index = 0;
            int stringLength = BitConverter.ToInt32(messageBytes, 0);
            index += 4;
            string proposedConversationName = Encoding.UTF8.GetString(messageBytes, index, stringLength);
            index += stringLength;
            while (index < messageBytes.Length)
            {
                stringLength = BitConverter.ToInt32(messageBytes, index);
                index += 4;
                namesOfParticipants.Add(Encoding.UTF8.GetString(messageBytes, index, stringLength));
                index += stringLength;
            }
            Console.WriteLine("DEBUG: trying to add conversation");
            byte[] reply = new byte[1];
            lock (allHandlers)
            {
                Conversation newConversation = chatSystem.addConversation(proposedConversationName, namesOfParticipants.ToArray());
                if (newConversation == null)
                {
                    reply[0] = 0;
                }
                else
                {
                    reply[0] = 1;
                    byte[] msg = newConversation.serialize(new ConcreteSerializer()).ToArray();
                    foreach (var handler in allHandlers.FindAll(h => namesOfParticipants.Contains(h.HandledUserName)))
                    {
                        handler.sendMessage(5, msg);
                    }
                }
            }
            handlerThread.sendMessage(1, reply);
        }
    }
}
