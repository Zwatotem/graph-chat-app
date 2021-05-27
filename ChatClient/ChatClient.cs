using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using ChatModel;
using ChatModel.Util;

namespace ChatClient
{
    public class ChatClient
    {
        private IPEndPoint serverEndPoint;
        private Socket socket;
        internal ReaderWriterLock readWriteLock;
        internal int lockTimeout;
        internal IClientChatSystem chatSystem;
        private byte[] inBuffer;
        internal bool responseReady;
        internal bool responseStatus;
        private int responseNumber;
        internal IIOSocketFacade socketFacade;
        internal int displayedConversationId;
        internal bool displayingConversation;
        internal bool displayingConversationsList;
        private bool goOn;

        ChatClient(string hostName, int portNumber)
        {
            serverEndPoint = new IPEndPoint(Dns.GetHostAddresses(hostName)[0], portNumber);
            readWriteLock = new ReaderWriterLock();
            lockTimeout = 10000;
            chatSystem = new ClientChatSystem();
            //Console.WriteLine("DEBUG: chatSystem created");
            inBuffer = new Byte[1024 * 1024];
            responseReady = false;
            displayedConversationId = -1;
            displayingConversationsList = false;
            displayingConversation = false;
            goOn = true;
        }

        private void listen()
        {
            try
            {
                //Console.WriteLine("DEBUG: listener thread started");
                int headerLength = BitConverter.GetBytes(4).Length + 1;
                byte[] headerBytes = new byte[headerLength];
                lock (this)
                {
                    while (goOn)
                    {
                        int bytesReceived = 0;
                        if (socket.Available == 0)
                        {
                            continue;
                        }
                        while (bytesReceived < headerLength)
                        {
                            bytesReceived += socket.Receive(headerBytes, bytesReceived, headerLength - bytesReceived, SocketFlags.None);
                        }
                        byte type = headerBytes[0];
                        int messageLength = BitConverter.ToInt32(headerBytes, 1);
                        bytesReceived = 0;
                        while (bytesReceived < messageLength)
                        {
                            bytesReceived += socket.Receive(inBuffer, bytesReceived, messageLength - bytesReceived, SocketFlags.None);
                        } //do poprawy, bufor moze byc za maly
                        if (type == (byte)1)
                        {
                            //Console.WriteLine("DEBUG: listener received boolean response");
                            responseStatus = (inBuffer[0] == (byte)0) ? false : true;
                            responseReady = true;
                            Monitor.Pulse(this);
                            Monitor.Wait(this);
                        }
                        else if (type == (byte)3)
                        {
                            //Console.WriteLine("DEBUG: listener received user to remove from conversation");
                            int conversationId = BitConverter.ToInt32(inBuffer, 0);
                            Conversation conversation = chatSystem.getConversation(conversationId);
                            string nameToRemove = Encoding.UTF8.GetString(inBuffer, 4, messageLength - 4);
                            bool result = false;
                            try
                            {
                                readWriteLock.AcquireWriterLock(lockTimeout);
                                result = chatSystem.leaveConversation(nameToRemove, conversationId);
                            }
                            finally
                            {
                                readWriteLock.ReleaseWriterLock();
                            }
                            if (!result)
                            {
                                Console.WriteLine("ERROR: something unexpected in {0}", "user to remove");
                            }
                        }
                        else if (type == (byte)4)
                        {
                            //Console.WriteLine("DEBUG: listener received user to add to conversation");
                            int conversationId = BitConverter.ToInt32(inBuffer, 0);
                            Conversation conversation = chatSystem.getConversation(conversationId);
                            string nameToAdd = Encoding.UTF8.GetString(inBuffer, 4, messageLength - 4);
                            if (chatSystem.getUser(nameToAdd) == null)
                            {
                                try
                                {
                                    readWriteLock.AcquireWriterLock(lockTimeout);
                                    chatSystem.addNewUser(nameToAdd);
                                }
                                finally
                                {
                                    readWriteLock.ReleaseWriterLock();
                                }
                            }
                            bool result = false;
                            try
                            {
                                readWriteLock.AcquireWriterLock(lockTimeout);
                                result = chatSystem.addUserToConversation(nameToAdd, conversationId);
                            }
                            finally
                            {
                                readWriteLock.ReleaseWriterLock();
                            }
                            if (!result)
                            {
                                Console.WriteLine("ERROR: something unexpected in {0}", "user to add");
                            }
                        }
                        else if (type == (byte)5)
                        {
                            //Console.WriteLine("DEBUG: listener received serialized conversation");
                            MemoryStream memStream = new MemoryStream(inBuffer, 0, messageLength);
                            Conversation result;
                            try
                            {
                                readWriteLock.AcquireWriterLock(lockTimeout);
                                result = chatSystem.addConversation(memStream, new ConcreteDeserializer());
                            }
                            finally
                            {
                                readWriteLock.ReleaseWriterLock();
                            }
                            if (result == null)
                            {
                                Console.WriteLine("ERROR: something unexpected in {0}", "serialized conversation");
                            }
                            else if(displayingConversationsList)
                            {
                                Console.WriteLine("{0}:\t{1}", result.ID, result.Name);
                            }
                        }
                        else if (type == (byte)6)
                        {
                            //Console.WriteLine("DEBUG: listener received serialized message");
                            int conversationId = BitConverter.ToInt32(inBuffer, 0);
                            MemoryStream memStream = new MemoryStream(inBuffer, 4, messageLength - 4);
                            Conversation conversation = chatSystem.getConversation(conversationId);
                            Message result = null;
                            if (conversation != null)
                            {
                                try
                                {
                                    readWriteLock.AcquireWriterLock(lockTimeout);
                                    result = conversation.addMessage(memStream, new ConcreteDeserializer());
                                    
                                }
                                finally
                                {
                                    readWriteLock.ReleaseWriterLock();
                                }
                            }
                            if (conversation == null || result == null)
                            {
                                Console.WriteLine("ERROR: something unexpected in {0}", "serialized message");
                            }
                            else if (displayingConversation && displayedConversationId == conversation.ID)
                            {
                                Console.WriteLine("{0} at {1} ID {2}:", result.Author, result.SentTime, result.ID);
                                Console.WriteLine(result.Content);
                            }
                        }
                    }
                    //Console.WriteLine("DEBUG: listener thread terminating");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown in listener: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async void workClient()
        {
            try
            {
                socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);
                socketFacade = new ConcreteIOSocketFacade(socket);
                //Console.WriteLine("DEBUG: socket connected");
                Thread listener = new Thread(this.listen);
                listener.Start();
                IPanelHandlerCreator factory = new ConcretePanelHandlerCreator();
                int decision = 10;
                while(goOn)
                {
                    IPanelHandler panelHandler = factory.createPanelHandler(decision);
                    decision = panelHandler.handle(this);
                }
                /*
                Console.Write("Give number from 0 to 6: ");
                int actionNumber = Convert.ToInt32(Console.ReadLine());
                while (actionNumber != 0)
                {
                    switch (actionNumber)
                    {
                        case 1: //create new user
                            await Task.Run(requestCreateNewUser);
                            break;
                        case 2: //logIn
                            await Task.Run(requestLogIn);
                            break;
                        case 3: //add conversation
                            await Task.Run(requestAddConversation);
                            break;
                        case 4: //add user to conversation
                            await Task.Run(requestAddUserToConversation);
                            break;
                        case 5: //leave conversation
                            await Task.Run(requestLeaveConversation);
                            break;
                        case 6: //send text message
                            await Task.Run(requestSendTextMessage);
                            break;
                        case 11: //test case
                            Console.Write("Conversations of klaus: ");
                            chatSystem.getUser("klaus").Conversations.ForEach(c => Console.WriteLine(c.Name));
                            Console.Write("Conversations of hans: ");
                            chatSystem.getUser("hans").Conversations.ForEach(c => Console.WriteLine(c.Name));
                            break;
                    }
                    Console.Write("Give number from 0 to 6: ");
                    actionNumber = Convert.ToInt32(Console.ReadLine());                   
                }
                requestDisconnect();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }

        public void requestCreateNewUser()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "add new user");
            string proposedName = null;
            Console.Write("Enter proposed name: ");
            proposedName = Console.ReadLine();
            byte[] content = Encoding.UTF8.GetBytes(proposedName);
            int contentLength = content.Length;
            byte[] header = new byte[5];
            header[0] = 1;
            Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "add new user");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                if (response)
                {
                    chatSystem.addNewUser(proposedName);
                }
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully added user: {0}", proposedName);
            }
            else
            {
                Console.WriteLine("Username already taken: {0}", proposedName);
            }
        }

        public void requestLogIn()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "logIn");
            string userName = null;
            Console.Write("Enter your user Name: ");
            userName = Console.ReadLine();
            byte[] content = Encoding.UTF8.GetBytes(userName);
            int contentLength = content.Length;
            byte[] header = new byte[5];
            header[0] = 2;
            Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "logIn");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                if (response)
                {
                    chatSystem.addNewUser(userName);
                    chatSystem.logIn(userName);
                }
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully loggedIn: {0}", userName);
            }
            else
            {
                Console.WriteLine("There is no such user");
            }
        }

        public void requestAddConversation()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "add conversation");
            List<byte> contentList = new List<byte>();
            Console.Write("Enter proposed conversation name: ");
            string conversationName = Console.ReadLine();
            foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(conversationName)))
            {
                contentList.Add(b);
            }
            foreach (byte b in Encoding.UTF8.GetBytes(conversationName))
            {
                contentList.Add(b);
            }
            Console.Write("With how many users? ");
            int usersToAdd = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < usersToAdd; ++i)
            {
                Console.WriteLine("Give next user's name: ");
                string userName = Console.ReadLine();
                foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(userName)))
                {
                    contentList.Add(b);
                }
                foreach (byte b in Encoding.UTF8.GetBytes(userName))
                {
                    contentList.Add(b);
                }
            }
            byte[] content = contentList.ToArray();
            int contentLength = content.Length;
            byte[] header = new byte[5];
            header[0] = 3;
            Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "add conversation");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully added conversation: {0}", conversationName);
            }
            else
            {
                Console.WriteLine("At least one of given users does not exist");
            }
        }

        public void requestAddUserToConversation()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "add user to conversation");
            string yourName = null;
            try
            {
                readWriteLock.AcquireReaderLock(lockTimeout);
                yourName = chatSystem.LoggedInName;
                if (yourName == null)
                {
                    Console.WriteLine("You must be logged in first!");
                    return;
                }
                Console.WriteLine("Here is the list of your conversations:");
                chatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}\t-\t{1}", c.Name, c.ID));
            }
            finally
            {
                readWriteLock.ReleaseReaderLock();
            }
            Console.Write("Give the id of conversation to which you want to add user: ");
            int conversationId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Give the user name of the user you want to add: ");
            string userName = Console.ReadLine();
            int contentLength = Encoding.UTF8.GetByteCount(userName) + 4;
            byte[] content = new byte[contentLength];
            Array.Copy(BitConverter.GetBytes(conversationId), 0, content, 0, 4);
            Array.Copy(Encoding.UTF8.GetBytes(userName), 0, content, 4, contentLength - 4);
            byte[] header = new byte[5];
            header[0] = 4;
            Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "add user to conversation");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully added user to conversation: {0}", userName);
            }
            else
            {
                Console.WriteLine("Given user does not exist");
            }
        }

        public void requestLeaveConversation()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "leave conversation");
            string yourName = null;
            try
            {
                readWriteLock.AcquireReaderLock(lockTimeout);
                yourName = chatSystem.LoggedInName;
                if (yourName == null)
                {
                    Console.WriteLine("You must be logged in first!");
                    return;
                }
                Console.WriteLine("Here is the list of your conversations:");
                chatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}\t-\t{1}", c.Name, c.ID));
            }
            finally
            {
                readWriteLock.ReleaseReaderLock();
            }
            Console.Write("Give the id of conversation you want to leave: ");
            int conversationId = Convert.ToInt32(Console.ReadLine());
            byte[] content = BitConverter.GetBytes(conversationId);
            byte[] header = new byte[5];
            header[0] = 5;
            Array.Copy(BitConverter.GetBytes(content.Length), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "leave conversation");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                if (response)
                {
                    chatSystem.getConversation(conversationId).Users.ForEach(u => chatSystem.leaveConversation(u.Name, conversationId));
                }
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully left conversation");
            }
            else
            {
                Console.WriteLine("You are not a member of that conversation");
            }
        }

        public void requestSendTextMessage()
        {
            Console.WriteLine("DEBUG: attempt to {0}", "send message");
            string yourName = null;
            try
            {
                readWriteLock.AcquireReaderLock(lockTimeout);
                yourName = chatSystem.LoggedInName;
                if (yourName == null)
                {
                    Console.WriteLine("You must be logged in first!");
                    return;
                }
                Console.WriteLine("Here is the list of your conversations:");
                chatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}\t-\t{1}", c.Name, c.ID));
            }
            finally
            {
                readWriteLock.ReleaseReaderLock();
            }
            Console.Write("Give the id of conversation where you want to send message: ");
            int conversationId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Here is the list of messages in your conversations:");
            try
            {
                readWriteLock.AcquireReaderLock(lockTimeout);
                foreach (var message in chatSystem.getConversation(conversationId).Messages)
                {
                    Console.WriteLine("{0}: {1}", message.ID, message.Content.getData());
                }
            }
            finally
            {
                readWriteLock.ReleaseReaderLock();
            }
            Console.Write("Give the id of the message you want to target: ");
            int messageId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the text of the message:");
            string text = Console.ReadLine();
            int contentLength = 9 + Encoding.UTF8.GetByteCount(text);
            byte[] content = new byte[contentLength];
            Array.Copy(BitConverter.GetBytes(conversationId), 0, content, 0, 4);
            Array.Copy(BitConverter.GetBytes(messageId), 0, content, 4, 4);
            content[8] = 1;
            Array.Copy(Encoding.UTF8.GetBytes(text), 0, content, 9, contentLength - 9);
            byte[] header = new byte[5];
            header[0] = 6;
            Array.Copy(BitConverter.GetBytes(content.Length), 0, header, 1, 4);
            socket.Send(header);
            socket.Send(content);
            Console.WriteLine("DEBUG: sending {0} request", "send message");
            bool response = false;
            lock (this)
            {
                while (!responseReady)
                {
                    Monitor.Wait(this);
                }
                response = responseStatus;
                responseReady = false;
                Monitor.Pulse(this);
            }
            if (response)
            {
                Console.WriteLine("Successfully sent message");
            }
            else
            {
                Console.WriteLine("Could not send the message");
            }
        }

        public void requestDisconnect()
        {
            goOn = false;
            byte[] request = new byte[5];
            request[0] = 0;
            Array.Copy(BitConverter.GetBytes(0), 0, request, 1, 4);
            socket.Send(request);
        }

        public static void Main(string[] args)
        {
            //ChatClient client = new ChatClient(args[0], Convert.ToInt32(args[1]));
            ChatClient client = new ChatClient("czamara.dyndns.org", 50000);
            client.workClient();
        }
    }
}
