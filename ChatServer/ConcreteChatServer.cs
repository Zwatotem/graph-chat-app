using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    /// <summary>
    /// Concrete implementation of IChatServer.
    /// </summary>
    public class ConcreteChatServer : IChatServer
    {
        private IPEndPoint ipEndPoint; //the local endpoint (basically ip address + port number) of the server
        private Socket socket; //socket used for accepting client connections
        private IServerChatSystem chatSystem; //chatSystem storing and modifying all information about conversations, messages and so forth
        private List<IClientHandler> handlers; //list of all handlers handling connected clients
        private int maxUsers; //number of clients simultaneously allowed to be connected
        private bool working;

        public ConcreteChatServer(string ipString, int portNumber, int capacity)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipString), portNumber);
            chatSystem = new ServerChatSystem();
            handlers = new List<IClientHandler>();
            maxUsers = capacity;
            working = false;
        }

        public bool Working { get => working; }

        public void startServer()
        {
            if (!working) //only if the server isn't working
            {
                working = true;
                Thread accepterThread = new Thread(acceptConnections); //thread for accepting connections
                accepterThread.Start();
            }           
        }

        public void shutdown()
        {
            working = false;
            socket.Close();
        }

        /// <summary>
        /// Accepts incoming connections from the clients, creates a handler for each one.
        /// </summary>
        private void acceptConnections()
        {
            try
            {
                Console.WriteLine("DEBUG: Accepting connections...");
                //creating socket and binding it to endpoint
                socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipEndPoint);
                socket.Listen(maxUsers); //we allow only as many users in backlog as max capacity of the server
                while (working)
                {
                    if (handlers.Count == maxUsers) //if the server is full
                    {
                        Thread.Sleep(1000); //wait one second, then check again
                        continue;
                    }
                    Socket newSocket = socket.Accept(); //accept a connection from the client
                    IClientHandler newHandler = new ConcreteClientHandler(chatSystem, newSocket, handlers, 5); //create a handler for it
                    lock (this) //prohibit interference from handlers
                    {
                        handlers.Add(newHandler); //add newly created handler to the list
                    }
                    newHandler.startWorking(); //start handler
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DEBUG: Exception thrown: {0}", ex.Message);
            }
            finally
            {
                socket.Dispose(); //release all reasources used by the socket
            }
        }
    }
}

/*
1. Single resposibility - it only has logic for accepting new clients (and necessary preparations before and cleaning after).
2. Open/closed - rather than changing this class, one can just implement the interface anew.
3. Liskov Substitution - all methods of interface (the contract) are properly implemented.
5. Dependency inversion - references to handlers and chat system within the class are to to interface types, not concrete classes.
*/

