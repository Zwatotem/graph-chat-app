using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatModel;

namespace ChatClient
{
    /// <summary>
    /// Main class of the client app.
    /// </summary>
    public class ChatClient
    {
        private IPEndPoint serverEndPoint;
        private Socket socket; //socket used to communicate
        internal IIOSocketFacade socketFacade; //facade simplifying the use of the socket
        internal ReaderWriterLock readWriteLock; //used to secure data of the local chat system
        internal int lockTimeout; //fixed value to set as readWriteLock's timeout
        internal IClientChatSystem chatSystem; //local chat system
        internal bool responseReady; //used by listener thread to communicate to main thread that response to request is ready
        internal bool responseStatus; //used to access this response
        internal int displayedConversationId; //id of currently displayed conversation
        internal bool displayingConversation; //true if a conversation is being displayed and thus updates are to be printed if received
        internal bool displayingConversationsList; //true if a conversations' list is being displayed and thus updates are to be printed if received
        private bool goOn; //true while working, set to false to stop

        ChatClient(string hostName, int portNumber)
        {
            serverEndPoint = new IPEndPoint(Dns.GetHostAddresses(hostName)[0], portNumber);
            readWriteLock = new ReaderWriterLock();
            lockTimeout = 10000;
            chatSystem = new ClientChatSystem();
            responseReady = false;
            displayedConversationId = -1; //unimportant for now
            displayingConversationsList = false;
            displayingConversation = false;
            goOn = true;
        }

        /// <summary>
        /// Executed by listener thread.
        /// </summary>
        private void listen()
        {
            try
            {
                int headerLength = BitConverter.GetBytes(4).Length + 1;
                byte[] headerBytes = new byte[headerLength];
                ITransmissionHandlerCreator factory = new ConcreteTransmissionHandlerCreator(); //will create context for strategy pattern
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
                        if (type == (byte)1) //received boolean response
                        {
                            responseStatus = (inBuffer[0] == (byte)0) ? false : true;
                            responseReady = true;
                            Monitor.Pulse(this);
                            Monitor.Wait(this);
                        }
                        else //received transmission of some sort
                        {
                            ITransmissionHandler transmissionHandler = factory.createTransmissionHandler(type); //create strategy pattern context
                            transmissionHandler.handle(this, inBuffer); //thanks to the pattern we don't worry here what transmission exactly it is
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
                socketFacade = new ConcreteIOSocketFacade(socket); //wrap socket with the facade to make using it easier
                Thread listener = new Thread(this.listen);
                listener.Start(); //start listener thread
                IPanelHandlerCreator factory = new ConcretePanelHandlerCreator(); //will create strategy pattern context
                int decision = 10; //start with decision 10, indicating displaying the welcome panel
                while (goOn)
                {
                    IPanelHandler panelHandler = factory.createPanelHandler(decision); //create strategy pattern context
                    decision = panelHandler.handle(this); //thanks to the pattern we don't worry here what panel we are to display and handle
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
            request[0] = 0; //type 0 is disconnect request
            Array.Copy(BitConverter.GetBytes(0), 0, request, 1, 4);
            socket.Send(request);
        }

        public static void Main(string[] args)
        {
            ChatClient client = new ChatClient(args[0], Convert.ToInt32(args[1])); //first arg - hostname, second - port nr
            client.workClient();
        }
    }
}

/*
This class is a decent example of compliance with SOLID:
1. It's resposibility is basically only being handling the connection with server.
2. It references interfaces where possible and delegeates specialized logic to specialized classes to makes extension viable over modification.
4. Interfaces used within are small (usually just one method) - as recommended by interface segregation.
5. Dependency inversion is realized in many places within, when rather than referencing concrete classes, abstract interfaces are being referenced.

Ideas behind design patterns used (factory and strategy) are analogus to those in chatServer for instance.
*/