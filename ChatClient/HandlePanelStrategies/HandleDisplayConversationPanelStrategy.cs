using System;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleDisplayConversationPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            Console.WriteLine("Type in a number to proceed:");
            Console.WriteLine("1 - add user\t2 - send message\t3 - show users' list\t4 - return to user panel\t0 - quit");
            Console.WriteLine();
            Console.WriteLine("Conversation name: {0}", client.chatSystem.getConversation(client.displayedConversationId).Name);
            try
            {
                client.readWriteLock.AcquireReaderLock(client.lockTimeout);
                foreach (var message in client.chatSystem.getConversation(client.displayedConversationId).Messages)
                {
                    Console.WriteLine("{0} at {1} ID: {2} reply: {3}", message.Author.Name, message.SentTime, message.ID, message.Parent.ID);
                    Console.WriteLine(message.Content.getData());
                }
                client.displayingConversation = true;
            }
            finally
            {
                client.readWriteLock.ReleaseReaderLock();
            }
            int decision;// = Convert.ToInt32(Console.ReadLine());
            bool isNum = int.TryParse(Console.ReadLine(), out decision);
            client.displayingConversation = false;
            if (!isNum || decision < 0 || decision > 4)
            {
                return 30;
            }
            if (decision == 4)
            {
                return 20;
            }
            return (decision == 0) ? decision : 3000 + decision;
        }
    }
}
