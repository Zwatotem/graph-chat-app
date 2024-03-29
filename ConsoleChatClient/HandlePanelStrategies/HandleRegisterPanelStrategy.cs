﻿using System;
using System.Text;
using System.Threading;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleRegisterPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if (!Console.IsOutputRedirected) {Console.Clear();}
            Console.Write("Enter proposed user name (or empty line to go back): ");
            string proposedName = Console.ReadLine();
            if (proposedName == "")
            {
                return 10;
            }
            byte[] message = Encoding.UTF8.GetBytes(proposedName);
            client.socketFacade.sendMessage(1, message);
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
                    client.chatSystem.addNewUser(proposedName);
                }
                Monitor.Pulse(client);
            }
            if (response)
            {
                Console.WriteLine("Successfully added user: {0}", proposedName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 10;
            }
            else
            {
                Console.WriteLine("Username already taken: {0}", proposedName);
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 1001;
            }
        }
    }
}
