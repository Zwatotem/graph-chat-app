using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatModel;

namespace ChatClient
{
    public class ChatClient
    {
        private IPEndPoint serverEndPoint;
        private Socket socket;
        private ClientChatSystem chatSystem;
        private byte[] inBuffer;
        private bool responseReady;
        private bool responseStatus;
        private int responseNumber;
        bool goOn;

        ChatClient(string serverIpText, int portNumber)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIpText), portNumber);
            chatSystem = new ClientChatSystem();
            Console.WriteLine("DEBUG: chatSystem created");
            inBuffer = new Byte[1024 * 1024];
            responseReady = false;
            goOn = true;
        }

        private void listen()
        {
            try
            {
                Console.WriteLine("DEBUG: listener thread started");
                int headerLength = BitConverter.GetBytes(4).Length;
                byte[] headerBytes = new byte[headerLength];
                lock (this)
                {
                    while (goOn)
                    {
                        int bytesReceived = 0;
                        bool continueFlag = false;
                        while (bytesReceived < headerLength)
                        {
                            bytesReceived += socket.Receive(headerBytes, bytesReceived, headerLength - bytesReceived, SocketFlags.None);
                            if (bytesReceived == 0)
                            {
                                continueFlag = true;
                                break;
                            }
                        }     
                        if (continueFlag)
                        {
                            continue;
                        }
                        int messageLength = BitConverter.ToInt32(headerBytes);
                        bytesReceived = 0;
                        while (bytesReceived < messageLength)
                        {
                            bytesReceived += socket.Receive(inBuffer, bytesReceived, messageLength - bytesReceived, SocketFlags.None);
                        } //do poprawy, bufor moze byc za maly
                        if (inBuffer[0] == (byte)0)
                        {
                            responseStatus = (inBuffer[1] == (byte)0) ? false : true;
                            responseReady = true;
                            Monitor.Pulse(this);
                            Monitor.Wait(this);
                        }
                    }
                    Console.WriteLine("DEBUG: listener thread terminating");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown in listener: {0}", ex.Message);
            }
        }

        private void workClient()
        {
            try
            {
                socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);
                Console.WriteLine("DEBUG: socket connected");
                Thread listener = new Thread(this.listen);
                listener.Start();
                int actionNumber = Convert.ToInt32(Console.ReadLine());
                while (actionNumber != 0)
                {
                    switch (actionNumber)
                    {
                        case 1: //create new user
                            bool response = false;
                            string proposedName = null;
                            while (!response)
                            {
                                proposedName = Console.ReadLine();
                                string request = "newUser:" + proposedName;
                                byte[] content = Encoding.UTF8.GetBytes(request);
                                int contentLength = content.Length;
                                byte[] header = BitConverter.GetBytes(contentLength);
                                socket.Send(header);
                                socket.Send(content);
                                Console.WriteLine("DEBUG: request sent");
                                lock (this)
                                {
                                    while (!responseReady)
                                    {
                                        Monitor.Wait(this);
                                    }
                                    response = responseStatus;
                                    responseReady = false;
                                    Monitor.Pulse(this);
                                }
                            }
                            chatSystem.addNewUser(proposedName);
                            Console.WriteLine("DEBUG: user added to chatSystem");
                            break;
                    }
                    actionNumber = Convert.ToInt32(Console.ReadLine());
                }
                //send sth let server know about disconnect
                string endMsgTxt = "disconnect:";
                byte[] endMsg = Encoding.UTF8.GetBytes(endMsgTxt);
                socket.Send(BitConverter.GetBytes(endMsg.Length));
                socket.Send(endMsg);
                goOn = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: {0}", ex.Message);
            }
            finally
            {
                socket.Dispose();
            }
        }

        public static void Main(string[] args)
        {
            ChatClient client = new ChatClient("192.168.42.225", 50000);
            client.workClient();
        }
    }
}
