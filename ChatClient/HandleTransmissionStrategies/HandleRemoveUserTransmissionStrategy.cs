using System;
using System.Text;
using ChatModel;

namespace ChatClient.HandleTransmissionStrategies
{
    public class HandleRemoveUserTransmissionStrategy : IHandleTransmissionStrategy
    {
        public void handle(ChatClient client, byte[] inBuffer)
        {
            int conversationId = BitConverter.ToInt32(inBuffer, 0);
            Conversation conversation = client.chatSystem.getConversation(conversationId);
            string nameToRemove = Encoding.UTF8.GetString(inBuffer, 4, inBuffer.Length - 4);
            bool result = false;
            try
            {
                client.readWriteLock.AcquireWriterLock(client.lockTimeout);
                result = client.chatSystem.leaveConversation(nameToRemove, conversationId);
            }
            finally
            {
                client.readWriteLock.ReleaseWriterLock();
            }
            if (!result)
            {
                Console.WriteLine("ERROR: something unexpected in {0}", "user to remove");
            }
        }
    }
}
