using System;
using System.Text;
using System.Threading;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleLogInPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            Console.Write("Enter your user name (or empty line to go back): ");
            string userName = Console.ReadLine();
            if (userName == "")
            {
                return 10;
            }
            byte[] message = Encoding.UTF8.GetBytes(userName);
            client.socketFacade.sendMessage(2, message);
            bool response = false;
            lock (client)
            {
                while (!client.responseReady)
                {
                    Monitor.Wait(client);
                }
                response = client.responseStatus;
                client.responseReady = false;
                if (response)
                {
                    client.chatSystem.addNewUser(userName);
                    client.chatSystem.logIn(userName);
                }
                Monitor.Pulse(client);
            }
            if (response)
            {
                Console.WriteLine("Successfully logged in: {0}", userName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 20;
            }
            else
            {
                Console.WriteLine("There is no such user or they are already logged in: {0}", userName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 1002;
            }
        }
    }
}
