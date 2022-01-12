using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleNewConversationPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if (!Console.IsOutputRedirected) {Console.Clear();}
            List<byte> contentList = new List<byte>();
            Console.Write("Enter proposed conversation name: ");
            string conversationName = Console.ReadLine();
            while(conversationName == "")
            {
                Console.WriteLine("Conversation name cannot be empty!");
                Console.Write("Enter proposed conversation name: ");
                conversationName = Console.ReadLine();
            }
            foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(conversationName)))
            {
                contentList.Add(b);
            }
            foreach (byte b in Encoding.UTF8.GetBytes(conversationName))
            {
                contentList.Add(b);
            }
            string userName = client.chatSystem.LoggedInName;
            foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(userName)))
            {
                contentList.Add(b);
            }
            foreach (byte b in Encoding.UTF8.GetBytes(userName))
            {
                contentList.Add(b);
            }
            Console.WriteLine("Who other than you is to be part of the conversation?");
            while (userName != "")
            {
                Console.WriteLine("Enter name of next user you wish to add to conversation (or empty line to stop): ");
                userName = Console.ReadLine();
                if (userName == "")
                {
                    break;
                }                
                foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(userName)))
                {
                    contentList.Add(b);
                }
                foreach (byte b in Encoding.UTF8.GetBytes(userName))
                {
                    contentList.Add(b);
                }
            }
            byte[] message = contentList.ToArray();
            client.socketFacade.sendMessage(3, message);
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
                Console.WriteLine("Successfully added conversation: {0}", conversationName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 20;
            }
            else
            {
                Console.WriteLine("At least one of entered users does not exist!");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 2001;
            }
        }
    }
}
