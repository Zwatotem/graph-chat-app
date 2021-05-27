using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleUserPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            string yourName = client.chatSystem.LoggedInName;
            Console.WriteLine("Logged in user: {0}", yourName);
            Console.WriteLine("Type in a number to proceed:");
            Console.WriteLine("1 - new conversation\t2 - leave conversation\t3 - display conversation\t0 - quit");
            Console.WriteLine("Your conversations (ID: Name):");
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
            int decision;// = Convert.ToInt32(Console.ReadLine());
            bool isNum = int.TryParse(Console.ReadLine(), out decision);
            client.displayingConversationsList = false;
            if (!isNum || decision < 0 || decision > 3)
            {
                return 20;
            }
            return (decision == 0) ? decision : 2000 + decision;
        }
    }
}
