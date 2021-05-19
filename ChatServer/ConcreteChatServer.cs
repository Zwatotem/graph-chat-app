using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public class ConcreteChatServer : IChatServer
    {
        private IPEndPoint ipEndPoint;
        private Socket socket;
        private ServerChatSystem chatSystem;
        private List<IClientHandler> handlers;
        private int maxUsers;
        private bool working;

        public ConcreteChatServer(string ipString, int portNumber, int capacity)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipString), portNumber);
            chatSystem = new ServerChatSystem();
            handlers = new List<IClientHandler>();
            maxUsers = capacity;
            working = false;
        }

        public void startServer()
        {
            if (!working)
            {
                working = true;
                Thread accepterThread = new Thread(acceptConnections);
                accepterThread.Start();
            }           
        }

        public bool isWorking()
        {
            return working;
        }

        public void shutdown()
        {
            working = false;
            socket.Close();
        }

        private void acceptConnections()
        {
            try
            {
                Console.WriteLine("DEBUG: Accepting connections...");
                socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipEndPoint);
                socket.Listen(maxUsers);
                while (working)
                {
                    if (handlers.Count == maxUsers)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    Socket newSocket = socket.Accept();
                    IClientHandler newHandler = new ConcreteClientHandler(chatSystem, newSocket, handlers, 5);
                    lock (this)
                    {
                        handlers.Add(newHandler);
                    }
                    newHandler.startWorking();
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
    }
}

