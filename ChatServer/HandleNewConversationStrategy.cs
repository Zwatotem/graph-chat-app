using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    class HandleNewConversationStrategy : IHandleRequestStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, HandlerThread handlerThread, byte[] messageBytes)
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
            lock (chatServer)
            {
                Conversation newConversation = chatSystem.addConversation(proposedConversationName, namesOfParticipants.ToArray());
                if (newConversation == null)
                {
                    reply[0] = 0;
                }
                else
                {
                    reply[0] = 1;
                    byte[] msg = newConversation.serialize().ToArray();
                    foreach (var handler in chatServer.Handlers.FindAll(h => namesOfParticipants.Contains(h.HandledUserName)))
                    {
                        handler.speak(5, msg);
                    }
                }
            }
            handlerThread.speak(1, reply);
        }
    }
}
