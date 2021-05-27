﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChatClient
{
    public class HandleLeaveConversationPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            string yourName = client.chatSystem.LoggedInName;
            Console.Write("Enter the ID of the conversation you wish to leave: ");
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
            Console.WriteLine("Are you sure you want to leave this conversation? (yes - confirm, otherwise - abort)");
            string confirmation = Console.ReadLine();
            if (confirmation != "yes")
            {
                return 20;
            }
            byte[] message = BitConverter.GetBytes(conversationId);
            client.socketFacade.sendMessage(5, message);
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
                    client.chatSystem.getConversation(conversationId).Users.ForEach(u => client.chatSystem.leaveConversation(u.Name, conversationId));
                }
                Monitor.Pulse(client);
            }
            if (response)
            {
                Console.WriteLine("Successfully left the conversation.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 20;
            }
            else
            {
                Console.WriteLine("An error ocurred while attempting to leave.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 20;
            }
        }
    }
}
