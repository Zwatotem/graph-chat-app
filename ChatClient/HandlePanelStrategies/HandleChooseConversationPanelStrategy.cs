using System;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleChooseConversationPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if (!Console.IsOutputRedirected) {Console.Clear();}
            string yourName = client.chatSystem.LoggedInName;
            Console.WriteLine("Enter the ID of the conversation you wish to display: ");
            try
            {
                client.readWriteLock.AcquireReaderLock(client.lockTimeout);
                client.chatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}:\t{1}", c.ID, c.Name));
                client.displayingConversationsList = true;
            }
            finally
            {
                client.readWriteLock.ReleaseReaderLock();
            }
            int conversationId;
            bool isNum = int.TryParse(Console.ReadLine(), out conversationId);
            client.displayingConversationsList = false;
            while (!isNum || client.chatSystem.getConversation(conversationId) == null)
            {
                Console.WriteLine("There is no such conversation!");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 20;
            }
            client.displayedConversationId = conversationId;
            return 30;
        }
    }
}
