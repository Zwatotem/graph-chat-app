using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using ChatServer.HandleStrategies;
/*
namespace ChatServer
{
    public class ConcreteListenerThread : IListenerThread
    {
        private Socket socket;      
        private Thread listener;
        IRequestHandler requestHandler;
        private int requestHeaderLength;
        private bool working;

        public ConcreteListenerThread(Socket socket, int headerLength, IRequestHandler requestHandler)
        {
            this.socket = socket;          
            this.listener = new Thread(this.listen);
            this.requestHeaderLength = headerLength;
            this.requestHandler = requestHandler;
        }

        public void startListening()
        {
            working = true;
            listener.Start();
        }

        private void listen()
        {
            Console.WriteLine("DEBUG: listener thread started");
            byte[] headerBytes = new byte[requestHeaderLength];
            while (working)
            {
                if (socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0)
                {
                    Console.WriteLine("DEBUG: socket closed ungracefully by client");
                    requestHandler.setHandleStrategy(new HandleDisconnectStrategy);
                }
                int bytesReceived = 0;
                while (bytesReceived < requestHeaderLength)
                {
                    bytesReceived += socket.Receive(headerBytes, bytesReceived, requestHeaderLength - bytesReceived, SocketFlags.None);
                }
                byte typeByte = headerBytes[0];
                int messageLength = BitConverter.ToInt32(headerBytes, 1);
                if (typeByte == 0)
                {
                    shutdown();
                }
                else
                {
                    IRequestHandler requestHandler = requestHandlerCreator.createRequestHandler(typeByte);
                    requestHandler.handleMessage(chatServer, chatSystem, this, receiveMessage(messageLength));
                }
            }
        }
    }
}
*/