using System;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
    public class ConcreteClientHandler : IClientHandler
    {
        private IIOSocketFacade socketFacade;
        private string handledUserName;
        private ServerChatSystem chatSystem;      
        private List<IClientHandler> allHandlers;
        private Thread listenerThread;
        private bool working;
        private IRequestHandlerCreator requestHandlerCreator;
        private int requestHeaderLength;

        public string HandledUserName
        {
            get => handledUserName;
            set => handledUserName = value;
        }

        public ConcreteClientHandler(ServerChatSystem chatSystem, Socket socket, List<IClientHandler> allHandlers, int headerLength)
        {
            Console.WriteLine("DEBUG: handler object created");
            this.chatSystem = chatSystem;
            this.socketFacade = new ConcreteIOSocketFacade(socket);
            this.allHandlers = allHandlers;                     
            this.requestHandlerCreator = new ConcreteRequestHandlerCreator();
            this.requestHeaderLength = headerLength;
            this.listenerThread = new Thread(this.listen);
            this.working = false;
            this.handledUserName = null;
        }

        public void startWorking()
        {
            if (!working)
            {
                working = true;
                listenerThread.Start();
            }            
        }

        public bool isWorking()
        {
            return working;
        }

        public void sendMessage(byte typeByte, byte[] message)
        {
            socketFacade.sendMessage(typeByte, message);
        }

        public void shutdown()
        {
            working = false;
            lock (allHandlers)
            {
                allHandlers.Remove(this);
            }
            socketFacade.shutdown();
        }

        private void listen()
        {
            Console.WriteLine("DEBUG: listener thread started");
            while (working)
            {
                byte typeByte = 0;
                byte[] messageBytes = null;
                try
                {
                    byte[] headerBytes = socketFacade.receiveMessage(requestHeaderLength);
                    typeByte = headerBytes[0];
                    int messageLength = BitConverter.ToInt32(headerBytes, 1);
                    messageBytes = socketFacade.receiveMessage(messageLength);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("DEBUG: SocketException thrown: {0}", ex.Message);
                    typeByte = 0;
                }
                IRequestHandler requestHandler = requestHandlerCreator.createRequestHandler(typeByte);
                requestHandler.handleRequest(allHandlers, chatSystem, this, messageBytes);
            }
        }

        
    }
}
