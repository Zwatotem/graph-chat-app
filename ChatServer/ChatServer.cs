using System;
using System.Net;
using System.Net.Sockets;
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
        private List<ClientHandler> handlers;
        private int maxUsers;

        public ChatServer(string ipString, int portNumber, int capacity)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipString), portNumber);
            chatSystem = new ServerChatSystem();
            handlers = new List<ClientHandler>();
            maxUsers = capacity;
        }

        public List<ClientHandler> Handlers
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
                socket.Listen(maxUsers);
                while (true)
                {
                    while (handlers.Count == maxUsers)
                    {
                        Thread.Sleep(1000);
                    }
                    Socket newSocket = socket.Accept();
                    ClientHandler newHandler = new ClientHandler(chatSystem, newSocket, this, 5);
                    lock (this)
                    {
                        handlers.Add(newHandler);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DEBUG: Exception thrown: {0}", ex.Message);
            }
            finally
            {              
                socket.Dispose();
            }
        }

        public void removeHandler(ClientHandler handler)
        {
            handlers.Remove(handler);
        }

        public static void Main(string[] args)
        {
            ChatServer chatServer = new ChatServer("192.168.42.225", 50000, 5);
            chatServer.acceptConnections();
        }
    }
}
    
