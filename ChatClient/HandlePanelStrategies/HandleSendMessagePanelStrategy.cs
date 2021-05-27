using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleSendMessagePanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            Console.WriteLine("Enter ID of the message to which you want to reply (-1 to not reply to any one): ");
            int messageId;// = Convert.ToInt32(Console.ReadLine());
            bool isNum = int.TryParse(Console.ReadLine(), out messageId);
            while (!isNum)
            {
                Console.Clear();
                Console.WriteLine("Enter ID of the message to which you want to reply (-1 to not reply to any one): ");
                isNum = int.TryParse(Console.ReadLine(), out messageId);
            }
            Console.WriteLine("Enter the text of your message (to end: ENTER + ^Z + ENTER):");
            StringBuilder messageBuilder = new StringBuilder();
            string line = null;
            while ((line = Console.ReadLine()) != null)
            {
                messageBuilder.Append(line);
                messageBuilder.Append('\n');
            }
            string messageText = messageBuilder.ToString();
            int messageLength = 9 + Encoding.UTF8.GetByteCount(messageText);
            byte[] message = new byte[messageLength];
            Array.Copy(BitConverter.GetBytes(client.displayedConversationId), 0, message, 0, 4);
            Array.Copy(BitConverter.GetBytes(messageId), 0, message, 4, 4);
            message[8] = 1;
            Array.Copy(Encoding.UTF8.GetBytes(messageText), 0, message, 9, messageLength - 9);
            client.socketFacade.sendMessage(6, message);
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
                Console.WriteLine("Successfully sent message.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 30;
            }
            else
            {
                Console.WriteLine("Could not send the message.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return 30;
            }
        }
    }
}
