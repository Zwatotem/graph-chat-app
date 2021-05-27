using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatModel;

namespace ChatClient
{
    public class ChatClient
    {
        private IPEndPoint serverEndPoint;
        private Socket socket;
        internal ReaderWriterLock readWriteLock;
        internal int lockTimeout;
        internal IClientChatSystem chatSystem;
        internal bool responseReady;
        internal bool responseStatus;
        internal IIOSocketFacade socketFacade;
        internal int displayedConversationId;
        internal bool displayingConversation;
        internal bool displayingConversationsList;
        private bool goOn;

        ChatClient(string hostName, int portNumber)
        {
            serverEndPoint = new IPEndPoint(Dns.GetHostAddresses(hostName)[0], portNumber);
            readWriteLock = new ReaderWriterLock();
            lockTimeout = 10000;
            chatSystem = new ClientChatSystem();
            responseReady = false;
            displayedConversationId = -1;
            displayingConversationsList = false;
            displayingConversation = false;
            goOn = true;
        }

        private void listen()
        {
            try
            {
                int headerLength = BitConverter.GetBytes(4).Length + 1;
                byte[] headerBytes = new byte[headerLength];
                ITransmissionHandlerCreator factory = new ConcreteTransmissionHandlerCreator();
                lock (this)
                {
                    while (goOn)
                    {
                        int bytesReceived = 0;
                        if (socket.Available == 0)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        while (bytesReceived < headerLength)
                        {
                            bytesReceived += socket.Receive(headerBytes, bytesReceived, headerLength - bytesReceived, SocketFlags.None);
                        }
                        byte type = headerBytes[0];
                        int messageLength = BitConverter.ToInt32(headerBytes, 1);
                        byte[] inBuffer = socketFacade.receiveMessage(messageLength);
                        if (type == (byte)1)
                        {
                            responseStatus = (inBuffer[0] == (byte)0) ? false : true;
                            responseReady = true;
                            Monitor.Pulse(this);
                            Monitor.Wait(this);
                        }
                        else
                        {
                            ITransmissionHandler transmissionHandler = factory.createTransmissionHandler(type);
                            transmissionHandler.handle(this, inBuffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown in listener: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void workClient()
        {
            try
            {
                socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);
                socketFacade = new ConcreteIOSocketFacade(socket);
                Thread listener = new Thread(this.listen);
                listener.Start();
                IPanelHandlerCreator factory = new ConcretePanelHandlerCreator();
                int decision = 10;
                while (goOn)
                {
                    IPanelHandler panelHandler = factory.createPanelHandler(decision);
                    decision = panelHandler.handle(this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }

        public void requestDisconnect()
        {
            goOn = false;
            byte[] request = new byte[5];
            request[0] = 0;
            Array.Copy(BitConverter.GetBytes(0), 0, request, 1, 4);
            socket.Send(request);
        }

        public static void Main(string[] args)
        {
            ChatClient client = new ChatClient(args[0], Convert.ToInt32(args[1]));
            //ChatClient client = new ChatClient("czamara.dyndns.org", 50000);
            client.workClient();
        }
    }
}
