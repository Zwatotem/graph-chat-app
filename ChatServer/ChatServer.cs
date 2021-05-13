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
                    Console.WriteLine(((IPEndPoint)newSocket.LocalEndPoint).Port);
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
            int headerLength = BitConverter.GetBytes(4).Length + 1;
            byte[] headerBytes = new byte[headerLength];
            bool work = true;
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
                byte messageType = headerBytes[0];
                int messageLength = BitConverter.ToInt32(headerBytes, 1);
                switch (messageType)
                {
                    case 0: //disconnect request
                        Console.WriteLine("DEBUG: {0} request received", "disconnect");
                        lock (this)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        work = false;
                        chatServer.removeMe(this);
                        break;
                    case 1: //add new user request
                        Console.WriteLine("DEBUG: {0} request received", "add new user");
                        handleAddNewUser(messageLength);
                        break;
                    case 2: //login request
                        Console.WriteLine("DEBUG: {0} request received", "logIn");
                        handleLogIn(messageLength);
                        break;
                    case 3: //add conversation request
                        Console.WriteLine("DEBUG: {0} request received", "add new conversation");
                        handleAddConversation(messageLength);
                        break;
                    case 4: //add user to conversation request
                        Console.WriteLine("DEBUG: {0} request received", "add user to conversation");
                        handleAddUserToConversation(messageLength);
                        break;
                }             
            }
        }

        public void speak(byte type, byte[] msg)
        {
            byte[] header = new byte[5];
            header[0] = type;
            Array.Copy(BitConverter.GetBytes(msg.Length), 0, header, 1, 4);
            lock (this)
            {
                socket.Send(header);
                socket.Send(msg);
            }
        }

        private void handleAddNewUser(int lengthToRead)
        {
            byte[] buffer = receiveMessage(lengthToRead);
            string proposedName = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("DEBUG: trying to add new user");
            User newUser = chatSystem.addNewUser(proposedName);       
            byte[] reply = new byte[1];
            reply[0] = (newUser == null) ? (byte)0 : (byte)1;
            speak(1, reply);
        }

        private void handleLogIn(int lengthToRead)
        {
            byte[] buffer = receiveMessage(lengthToRead);
            string userName = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("DEBUG: requested logIn");
            User user = chatSystem.getUser(userName);
            byte[] reply = new byte[1];
            if (user == null)
            {
                reply[0] = 0;
            }
            else
            {
                reply[0] = 1;
                handledUserName = userName;
                foreach (var conversation in user.getConversations())
                {
                    byte[] msg = conversation.serialize().ToArray();
                    speak(5, msg);
                }
            }
            speak(1, reply);
        }

        private void handleAddConversation(int lengthToRead)
        {
            byte[] buffer = receiveMessage(lengthToRead);
            string proposedConversationName = null;
            List<string> namesOfParticipants = new List<string>();
            int index = 0;
            int stringLength = BitConverter.ToInt32(buffer, 0);
            index += 4;
            proposedConversationName = Encoding.UTF8.GetString(buffer, index, stringLength);
            index += stringLength;
            while (index < buffer.Length)
            {
                stringLength = BitConverter.ToInt32(buffer, index);
                index += 4;
                namesOfParticipants.Add(Encoding.UTF8.GetString(buffer, index, stringLength));
                index += stringLength;
            }
            Console.WriteLine("DEBUG: trying to add conversation");
            Conversation newConversation = chatSystem.addConversation(proposedConversationName, namesOfParticipants.ToArray());
            byte[] reply = new byte[1];
            if (newConversation == null)
            {
                reply[0] = 0;
            }
            else
            {
                reply[0] = 1;
                byte[] msg = newConversation.serialize().ToArray();
                foreach (var handler in chatServer.Handlers.FindAll(h => namesOfParticipants.Contains(h.handledUserName)))
                {
                    handler.speak(5, msg);
                }
            }
            speak(1, reply);
        }

        public void handleAddUserToConversation(int lengthToRead)
        {
            byte[] buffer = receiveMessage(lengthToRead);
            int conversationId = BitConverter.ToInt32(buffer, 0);
            Conversation conversation = chatSystem.getConversation(conversationId);
            string nameToAdd = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
            Console.WriteLine("DEBUG: trying to add user to conversation");
            byte[] reply = new byte[1];
            if(chatSystem.addUserToConversation(nameToAdd, conversationId))
            {
                reply[0] = 1;
                byte[] msg = buffer;
                foreach (var handler in chatServer.Handlers.FindAll(h => conversation.getUsers().Exists(u => u.getName() == h.handledUserName)))
                {
                    handler.speak(4, msg);
                }
            }
            else
            {
                reply[0] = 0;
            }
            speak(1, reply);
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
    }
}
