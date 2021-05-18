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
                    while (handlers.Count == 5)
                    {
                        Thread.Sleep(1000);
                    }
                    Socket newSocket = socket.Accept();
                    //Console.WriteLine(((IPEndPoint)newSocket.LocalEndPoint).Port);
                    HandlerThread newHandler = new HandlerThread(chatSystem, newSocket, this);
                    lock (this)
                    {
                        handlers.Add(newHandler);
                    }
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
}
    
