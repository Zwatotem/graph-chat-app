using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public class ChatServer
    {
        private IPEndPoint ipEndPoint;
        private Socket socket;
        private ServerChatSystem chatSystem;
        private List<HandlerThread> handlers;

        public ChatServer(string ipString, int portNumber)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipString), portNumber);
            chatSystem = new ServerChatSystem();
            handlers = new List<HandlerThread>();
        }

        public List<HandlerThread> Handlers
        {
            get
            {
                return handlers;
            }
        }   

        public void acceptConnections()
        {
            try
            {
                socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipEndPoint);
                socket.Listen(5);
                while (true)
                {
                    Socket newSocket = socket.Accept();
                    HandlerThread newHandler = new HandlerThread(chatSystem, newSocket, this);
                    handlers.Add(newHandler);
                }
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

        public void removeMe(HandlerThread handler)
        {
            handlers.Remove(handler);
        }

        public static void Main(string[] args)
        {
            ChatServer chatServer = new ChatServer("192.168.42.225", 50000);
            chatServer.acceptConnections();
        }
    }

    public class HandlerThread
    {
        private Socket socket;
        private string handledUserName;
        private ServerChatSystem chatSystem;
        private Thread listenerThread;
        private ChatServer chatServer;

        public HandlerThread(ServerChatSystem chatSystem, Socket socket, ChatServer chatServer)
        {
            Console.WriteLine("DEBUG: handler object created");
            this.chatSystem = chatSystem;
            this.socket = socket;
            this.chatServer = chatServer;
            this.listenerThread = new Thread(this.listen);
            this.listenerThread.Start();
            
        }

        public void listen()
        {
            Console.WriteLine("DEBUG: listener thread started");
            int headerLength = BitConverter.GetBytes(4).Length;
            byte[] headerBytes = new byte[headerLength];
            bool work = true;
            while (work) //dodac warunek odebrania wiadomosci disconnect
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
                byte[] inBuffer = new byte[messageLength];
                bytesReceived = 0;
                while (bytesReceived < messageLength)
                {
                    bytesReceived += socket.Receive(inBuffer, bytesReceived, messageLength - bytesReceived, SocketFlags.None);
                }
                string message = Encoding.UTF8.GetString(inBuffer);
                int pos = message.IndexOf(':');
                string command = message.Substring(0, pos);
                if (command == "newUser")
                {
                    string proposedName = message.Substring(pos + 1);
                    User newUser = chatSystem.addNewUser(proposedName);
                    if (newUser == null)
                    {
                        byte[] msg = { 0, 0 };
                        this.speak(msg);
                    }
                    else
                    {
                        byte[] msg = { 0, 1 };
                        Console.WriteLine("DEBUG: user added: {0}", newUser.getName());
                        this.speak(msg);
                    }
                }
                else if (command == "disconnect")
                {
                    Console.WriteLine("DEBUG: closing socket");
                    lock (this)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    work = false;
                    chatServer.removeMe(this);
                }
            }
        }

        public void speak(byte[] msg)
        {
            lock (this)
            {
                byte[] header = BitConverter.GetBytes(msg.Length);
                socket.Send(header);
                socket.Send(msg);
            }
        }
    }
}
