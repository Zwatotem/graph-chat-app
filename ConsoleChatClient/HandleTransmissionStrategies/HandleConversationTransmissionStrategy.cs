using System;
using System.IO;
using ChatModel;
using ChatModel.Util;

namespace ChatClient.HandleTransmissionStrategies
{
    public class HandleConversationTransmissionStrategy : IHandleTransmissionStrategy
    {
        public void handle(ChatClient client, byte[] inBuffer)
        {
            MemoryStream memStream = new MemoryStream(inBuffer, 0, inBuffer.Length);
            Conversation result;
            try
            {
                client.readWriteLock.AcquireWriterLock(client.lockTimeout);
                result = client.chatSystem.addConversation(memStream, new ConcreteDeserializer());
            }
            finally
            {
                client.readWriteLock.ReleaseWriterLock();
            }
            if (result == null)
            {
                Console.WriteLine("ERROR: something unexpected in {0}", "serialized conversation");
            }
            else if (client.displayingConversationsList)
            {
                Console.WriteLine("{0}:\t{1}", result.ID, result.Name);
            }
        }
    }
}
