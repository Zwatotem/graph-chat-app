using System;
using System.IO;
using ChatModel;
using ChatModel.Util;

namespace ChatClient.HandleTransmissionStrategies
{
    public class HandleMessageTransmissionStrategy : IHandleTransmissionStrategy
    {
        public void handle(ChatClient client, byte[] inBuffer)
        {
            int conversationId = BitConverter.ToInt32(inBuffer, 0);
            MemoryStream memStream = new MemoryStream(inBuffer, 4, inBuffer.Length - 4);
            Conversation conversation = client.chatSystem.getConversation(conversationId);
            Message result = null;
            if (conversation != null)
            {
                try
                {
                    client.readWriteLock.AcquireWriterLock(client.lockTimeout);
                    result = conversation.addMessage(memStream, new ConcreteDeserializer());

                }
                finally
                {
                    client.readWriteLock.ReleaseWriterLock();
                }
            }
            if (conversation == null || result == null)
            {
                Console.WriteLine("ERROR: something unexpected in {0}", "serialized message");
            }
            else if (client.displayingConversation && client.displayedConversationId == conversation.ID)
            {
                Console.WriteLine("{0} at {1} ID: {2} reply: {3}", result.Author.Name, result.SentTime, result.ID, result.Parent.ID);
                Console.WriteLine(result.Content.getData());
            }
        }
    }
}
