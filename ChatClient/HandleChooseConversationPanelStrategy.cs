using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class HandleChooseConversationPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
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
            int conversationId = Convert.ToInt32(Console.ReadLine());
            client.displayingConversationsList = false;
            while (client.chatSystem.getConversation(conversationId) == null)
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
