using System;
using System.Text;
using System.Threading;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleAddUserPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if (!Console.IsOutputRedirected) {Console.Clear();}
            Console.Write("Enter the user name of the user you wish to add to conversation: ");
            string userName = Console.ReadLine();
            int messageLength = Encoding.UTF8.GetByteCount(userName) + 4;
            byte[] message = new byte[messageLength];
            Array.Copy(BitConverter.GetBytes(client.displayedConversationId), 0, message, 0, 4);
            Array.Copy(Encoding.UTF8.GetBytes(userName), 0, message, 4, messageLength - 4);
            client.socketFacade.sendMessage(4, message);
            bool response = false;
            lock (client)
            {
                while (!client.responseReady)
                {
                    Monitor.Wait(client);
                }
                response = client.responseStatus;
                client.responseReady = false;
                Monitor.Pulse(client);
            }
            if (response)
            {
                Console.WriteLine("Successfully added user to conversation: {0}", userName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 30;
            }
            else
            {
                Console.WriteLine("Entered user does not exist.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 3001;
            }
        }
    }
}
