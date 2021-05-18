using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer
{
    class HandleSendMessageStrategy : IHandleRequestStrategy
    {
        public void handleMessage(ChatServer chatServer, ChatSystem chatSystem, HandlerThread handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "send message");
            int conversationId = BitConverter.ToInt32(messageBytes, 0);
            int targetedMessageId = BitConverter.ToInt32(messageBytes, 4);
            MessageContent content = ContentFactory.getContent(messageBytes, 8);
            byte[] reply = new byte[1];
            if (content == null)
            {
                reply[0] = 0;
            }
            else
            {
                Console.WriteLine("DEBUG: trying to send message");
                lock (chatServer)
                {
                    Message sentMessage = chatSystem.sendMessage(conversationId, handlerThread.HandledUserName, targetedMessageId, content, DateTime.Now);
                    if (sentMessage != null)
                    {
                        reply[0] = 1;
                        byte[] serializedBytes = sentMessage.serialize().ToArray(); //serialization must work properly for this one
                        byte[] msg = new byte[serializedBytes.Length + 4];
                        Array.Copy(BitConverter.GetBytes(conversationId), 0, msg, 0, 4);
                        Array.Copy(serializedBytes, 0, msg, 4, serializedBytes.Length);
                        Conversation conversation = chatSystem.getConversation(conversationId);
                        foreach (var handler in chatServer.Handlers.FindAll(h => conversation.getUsers().Exists(u => u.getName() == h.HandledUserName)))
                        {
                            handler.speak(6, msg);
                        }
                    }
                    else
                    {
                        reply[0] = 0;
                    }
                }
            }
            handlerThread.speak(1, reply);
        }
    }
}
