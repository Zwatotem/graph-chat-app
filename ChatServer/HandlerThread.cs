using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using ChatModel;

namespace ChatServer
{
    public class HandlerThread
    {
        private Socket socket;
        private string handledUserName;
        private ServerChatSystem chatSystem;
        private Thread listenerThread;
        private ChatServer chatServer;
        private bool work;
        private IRequestHandlerCreator requestHandlerCreator;

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

        public HandlerThread(ServerChatSystem chatSystem, Socket socket, ChatServer chatServer)
        {
            Console.WriteLine("DEBUG: handler object created");
            this.chatSystem = chatSystem;
            this.socket = socket;
            this.chatServer = chatServer;
            this.listenerThread = new Thread(this.listen);
            this.listenerThread.Start();
            this.work = true;
            this.requestHandlerCreator = new ConcreteRequestHandlerCreator();

        }

        public void listen()
        {
            Console.WriteLine("DEBUG: listener thread started");
            int headerLength = BitConverter.GetBytes(4).Length + 1;
            byte[] headerBytes = new byte[headerLength];
            while (work) //add check for ungracefull disconnect on client side
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
                    continue; //here goes the check probably
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

        private byte[] receiveMessage(int length)
        {
            byte[] buffer = new byte[length];
            int bytesReceived = 0;
            while (bytesReceived < length)
            {
                bytesReceived += socket.Receive(buffer, bytesReceived, length - bytesReceived, SocketFlags.None);
            }
            return buffer;
        }

        public void speak(byte type, byte[] msg)
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
            Console.WriteLine("DEBUG: {0} request received", "disconnect");
            lock (chatServer)
            {
                chatServer.removeMe(this);
            }
            lock (this)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            work = false;
        }
    }
}
