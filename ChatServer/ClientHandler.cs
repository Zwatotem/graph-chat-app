using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using ChatModel;
using ChatServer.HandleStrategies;

namespace ChatServer
{
    public class ClientHandler
    {
        private Socket socket;
        private string handledUserName;
        private ServerChatSystem chatSystem;
        private Thread listenerThread;
        private ChatServer chatServer;
        private bool work;
        private IRequestHandlerCreator requestHandlerCreator;
        private int requestHeaderLength;

        public string HandledUserName
        {
            get
            {
                return handledUserName;
            }
            set
            {
                handledUserName = value;
            }
        }

        public ClientHandler(ServerChatSystem chatSystem, Socket socket, ChatServer chatServer, int headerLength)
        {
            Console.WriteLine("DEBUG: handler object created");
            this.chatSystem = chatSystem;
            this.socket = socket;
            this.chatServer = chatServer;
            this.listenerThread = new Thread(this.listen);
            this.listenerThread.Start();
            this.work = true;
            this.requestHandlerCreator = new ConcreteRequestHandlerCreator();
            this.requestHeaderLength = headerLength;

        }

        public void listen()
        {
            Console.WriteLine("DEBUG: listener thread started");
            while (work)
            {
                byte typeByte = 0;
                byte[] messageBytes = null;
                try
                {
                    byte[] headerBytes = receiveMessage(requestHeaderLength);
                    typeByte = headerBytes[0];
                    int messageLength = BitConverter.ToInt32(headerBytes, 1);
                    messageBytes = receiveMessage(messageLength);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("DEBUG: SocketException thrown: {0}", ex.Message);
                    typeByte = 0;
                }
                IRequestHandler requestHandler = requestHandlerCreator.createRequestHandler(typeByte);
                requestHandler.handleMessage(chatServer, chatSystem, this, messageBytes);
            }
        }

        private byte[] receiveMessage(int length)
        {
            if (length == 0)
            {
                return null;
            }
            byte[] buffer = new byte[length];
            int bytesReceived = 0;
            while (bytesReceived < length)
            {
                bytesReceived += socket.Receive(buffer, bytesReceived, length - bytesReceived, SocketFlags.None);
            }
            return buffer;
        }

        public void sendMessage(byte type, byte[] msg)
        {
            byte[] header = new byte[5];
            header[0] = type;
            Array.Copy(BitConverter.GetBytes(msg.Length), 0, header, 1, 4);
            lock (this)
            {
                if (socket.Connected)
                {
                    socket.Send(header);
                    socket.Send(msg);
                }
            }
        }

        public void shutdown()
        {
            work = false;
            lock (chatServer)
            {
                chatServer.removeHandler(this);
            }
            lock (this)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }           
        }
    }
}
