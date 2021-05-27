using System;
using ChatModel;

namespace ChatClient
{
    public class HandleUsersListPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            try
            {
                client.readWriteLock.AcquireReaderLock(client.lockTimeout);
                Conversation conversation = client.chatSystem.getConversation(client.displayedConversationId);
                Console.WriteLine("List of users of conversation {0}:", conversation.Name);
                conversation.Users.ForEach(u => Console.WriteLine(u.Name));
            }
            finally
            {
                client.readWriteLock.ReleaseReaderLock();
            }
            Console.WriteLine("1 - refresh, otherwise - go back");
            int decision = Convert.ToInt32(Console.ReadLine());
            return (decision == 1) ? 3003 : 30;
        }
    }
}
