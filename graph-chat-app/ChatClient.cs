using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ChatModel;
using ChatModel.Util;
using System.Windows.Threading;

namespace GraphChatApp
{
	public class ChatClient
	{
		private IPEndPoint serverEndPoint;
		private Socket socket;
		private ReaderWriterLock readWriteLock;
		private Dispatcher dispatcher;
		private int lockTimeout;
		private ClientChatSystem chatSystem;
		private byte[] inBuffer;
		private bool responseReady;
		private bool responseStatus;
		private int responseNumber;
		bool goOn;

		public ClientChatSystem ChatSystem { get => chatSystem; set => chatSystem = value; }
		public Dispatcher Dispatcher
		{
			get => dispatcher;
			set => dispatcher = value;
		}

		public event EventHandler<SuccessfullyRegisteredEventArgs> SuccessfullyRegistered = (sender, e) => { };
		public event EventHandler<SuccessfullyLoggededEventArgs> SuccessfullyLogged = (sender, e) => { };
		public event EventHandler<SuccessfullyAddedConversationEventArgs> SuccessfullyAddedConversation = (sender, e) => { };


		public ChatClient(string serverIpText, int portNumber, Dispatcher dispatcher)
		{
			serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIpText), portNumber);
			readWriteLock = new ReaderWriterLock();
			lockTimeout = 10000;
			ChatSystem = new ClientChatSystem();
			Console.WriteLine("DEBUG: chatSystem created");
			this.dispatcher = dispatcher;
			inBuffer = new Byte[1024 * 1024];
			responseReady = false;
			goOn = true;
		}

		private void listen()
		{
			try
			{
				Console.WriteLine("DEBUG: listener thread started");
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
							Console.WriteLine("DEBUG: listener received boolean response");
							responseStatus = (inBuffer[0] == (byte)0) ? false : true;
							responseReady = true;
							Monitor.Pulse(this);
							Monitor.Wait(this);
						}
						else if (type == (byte)3)
						{
							Console.WriteLine("DEBUG: listener received user to remove from conversation");
							int conversationId = BitConverter.ToInt32(inBuffer, 0);
							Conversation conversation = ChatSystem.getConversation(conversationId);
							string nameToRemove = Encoding.UTF8.GetString(inBuffer, 4, messageLength - 4);
							bool result = false;
							try
							{
								readWriteLock.AcquireWriterLock(lockTimeout);
								result = ChatSystem.leaveConversation(nameToRemove, conversationId);
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
							Console.WriteLine("DEBUG: listener received user to add to conversation");
							int conversationId = BitConverter.ToInt32(inBuffer, 0);
							Conversation conversation = ChatSystem.getConversation(conversationId);
							string nameToAdd = Encoding.UTF8.GetString(inBuffer, 4, messageLength - 4);
							if (ChatSystem.getUser(nameToAdd) == null)
							{
								try
								{
									readWriteLock.AcquireWriterLock(lockTimeout);
									ChatSystem.addNewUser(nameToAdd);
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
								result = ChatSystem.addUserToConversation(nameToAdd, conversationId);
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
							// Someone added you I guess
							//Console.WriteLine("DEBUG: listener received serialized conversation");
							MemoryStream memStream = new MemoryStream(inBuffer, 0, messageLength);
							Conversation result;
							try
							{
								readWriteLock.AcquireWriterLock(lockTimeout);
								result = ChatSystem.addConversation(memStream, new ConcreteDeserializer());
							}
							finally
							{
								readWriteLock.ReleaseWriterLock();
							}
							if (result == null)
							{
								//Console.WriteLine("ERROR: something unexpected in {0}", "serialized conversation");
							}
							else
							{
								SuccessfullyAddedConversation(this, new(result));
							}
						}
						else if (type == (byte)6)
						{
							Console.WriteLine("DEBUG: listener received serialized message");
							int conversationId = BitConverter.ToInt32(inBuffer, 0);
							MemoryStream memStream = new MemoryStream(inBuffer, 4, messageLength - 4);
							Conversation conversation = ChatSystem.getConversation(conversationId);
							Message result = null;
							if (conversation != null)
							{
								this.Dispatcher.Invoke(() =>
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
								);
							}
							if (conversation == null || result == null)
							{
								Console.WriteLine("ERROR: something unexpected in {0}", "serialized message");
							}
						}
					}
					Console.WriteLine("DEBUG: listener thread terminating");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception thrown in listener: {0}", ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}

		public void workClient()
		{
			try
			{
				socket = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(serverEndPoint);
				Console.WriteLine("DEBUG: socket connected");
				Thread listener = new Thread(this.listen);
				listener.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception thrown: {0}", ex.Message);
				Console.WriteLine(ex.StackTrace);
				socket.Dispose();
				throw;
			}
		}

		public async void requestCreateNewUser(object sender, UserRegisteredEventArgs args)
		{
			Console.WriteLine("DEBUG: attempt to {0}", "add new user");
			string proposedName = args.Username;
			byte[] content = Encoding.UTF8.GetBytes(proposedName);
			int contentLength = content.Length;
			byte[] header = new byte[5];
			header[0] = 1;
			Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
			socket.Send(header);
			socket.Send(content);
			Console.WriteLine("DEBUG: sending {0} request", "add new user");
			IUser addedUser = null;
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					if (rsp)
					{
						addedUser = ChatSystem.addNewUser(proposedName);
					}
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				SuccessfullyRegistered.Invoke(this, new(addedUser));
				Console.WriteLine("Successfully added user: {0}", proposedName);
			}
			else
			{
				Console.WriteLine("Username already taken: {0}", proposedName);
			}
		}

		public async void requestLogIn(object sender, UserLoggedEventArgs args)
		{
			Console.WriteLine("DEBUG: attempt to {0}", "logIn");
			string userName = args.Username;
			Console.Write("Enter your user Name: ");
			byte[] content = Encoding.UTF8.GetBytes(userName);
			int contentLength = content.Length;
			byte[] header = new byte[5];
			header[0] = 2;
			Array.Copy(BitConverter.GetBytes(contentLength), 0, header, 1, 4);
			socket.Send(header);
			socket.Send(content);
			Console.WriteLine("DEBUG: sending {0} request", "logIn");
			User loggedUser = null;
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					if (rsp)
					{
						var loggedUser = ChatSystem.addNewUser(userName);
						ChatSystem.logIn(userName);
					}
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				//Console.WriteLine("Successfully loggedIn: {0}", userName);
				SuccessfullyLogged(this, new(loggedUser));
			}
			else
			{
				//Console.WriteLine("There is no such user");
			}
		}

		public async void requestAddConversation(object sender, ConversationAddedEventArgs args)
		{
			Console.WriteLine("DEBUG: attempt to {0}", "add conversation");
			List<byte> contentList = new List<byte>();
			Console.Write("Enter proposed conversation name: ");
			string conversationName = args.ConversationName;
			foreach (byte b in BitConverter.GetBytes(Encoding.UTF8.GetByteCount(conversationName)))
			{
				contentList.Add(b);
			}
			foreach (byte b in Encoding.UTF8.GetBytes(conversationName))
			{
				contentList.Add(b);
			}
			Console.Write("With how many users? ");
			int usersToAdd = args.Users.Length;
			for (int i = 0; i < usersToAdd; ++i)
			{
				Console.WriteLine("Give next user's name: ");
				string userName = args.Users[i];
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
			//Console.WriteLine("DEBUG: sending {0} request", "add conversation");
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				//Console.WriteLine("Successfully added conversation: {0}", conversationName);
				try
				{
					readWriteLock.AcquireReaderLock(lockTimeout);
					var conv = ChatSystem.Conversations.Where(x => x.Value.Name == conversationName).Select(x => x.Value).First();
					SuccessfullyAddedConversation(this, new(conv));
				}
				finally
				{
					readWriteLock.ReleaseReaderLock();
				}
			}
			else
			{
				//Console.WriteLine("At least one of given users does not exist");
			}
		}

		public async void requestAddUserToConversation()
		{
			Console.WriteLine("DEBUG: attempt to {0}", "add user to conversation");
			string yourName = null;
			bool loggedIn = await Task.Run(() =>
			{
				try
				{
					readWriteLock.AcquireReaderLock(lockTimeout);
					yourName = ChatSystem.getUserName();
					if (yourName == null)
					{
						Console.WriteLine("You must be logged in first!");
						return false;
					}
					Console.WriteLine("Here is the list of your conversations:");
					ChatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}\t-\t{1}", c.Name, c.ID));
				}
				finally
				{
					readWriteLock.ReleaseReaderLock();
				}
				return true;
			});
			if (!loggedIn)
			{
				return;
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
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				Console.WriteLine("Successfully added user to conversation: {0}", userName);
			}
			else
			{
				Console.WriteLine("Given user does not exist");
			}
		}

		public async void requestLeaveConversation()
		{
			Console.WriteLine("DEBUG: attempt to {0}", "leave conversation");
			string yourName = null;
			bool loggedIn = await Task.Run(() =>
			{
				try
				{
					readWriteLock.AcquireReaderLock(lockTimeout);
					yourName = ChatSystem.getUserName();
					if (yourName == null)
					{
						Console.WriteLine("You must be logged in first!");
						return false;
					}
					Console.WriteLine("Here is the list of your conversations:");
					ChatSystem.getUser(yourName).Conversations.ForEach(c => Console.WriteLine("{0}\t-\t{1}", c.Name, c.ID));
				}
				finally
				{
					readWriteLock.ReleaseReaderLock();
				}
				return true;
			});
			if (!loggedIn)
			{
				return;
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
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					if (rsp)
					{
						ChatSystem.leaveConversation(yourName, conversationId);
					}
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				Console.WriteLine("Successfully left conversation");
			}
			else
			{
				Console.WriteLine("You are not a member of that conversation");
			}
		}

		public async void requestSendTextMessage(object sender, SendMessageEventArgs args)
		{
			Console.WriteLine("DEBUG: attempt to {0}", "send message");
			string yourName = args.MessageSender.Name;
			int conversationId = args.ConversationID;
			int messageId = args.ParentMessageID;
			string text = args.MessageContent;
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
			bool response = await Task.Run(() =>
			{
				lock (this)
				{
					while (!responseReady)
					{
						Monitor.Wait(this);
					}
					bool rsp = responseStatus;
					responseReady = false;
					Monitor.Pulse(this);
					return rsp;
				}
			});
			if (response)
			{
				//Console.WriteLine("Successfully sent message");
			}
			else
			{
				//Console.WriteLine("Could not send the message");
			}
		}

		public void requestDisconnect()
		{
			Console.WriteLine("DEBUG: attempt to {0}", "disconnect");
			goOn = false;
			byte[] request = new byte[5];
			request[0] = 0;
			Array.Copy(BitConverter.GetBytes(0), 0, request, 1, 4);
			socket.Send(request);
			socket.Dispose();
		}
	}

	public class SendMessageEventArgs
	{
		public IUser MessageSender;
		public int ConversationID;
		public int ParentMessageID;
		public string MessageContent;
		public SendMessageEventArgs(IUser user, int conv, string content)
		{
			MessageSender = user;
			ConversationID = conv;
			ParentMessageID = -1;
			MessageContent = content;
		}
		public SendMessageEventArgs(IUser user, int conv, int target, string content)
			: this(user, conv, content)
		{
			ParentMessageID = target;
		}
	}

	public class SuccessfullyAddedConversationEventArgs
	{
		Conversation addedConversation;
		public Conversation AddedConversation { get => addedConversation; set => addedConversation = value; }
		public SuccessfullyAddedConversationEventArgs(Conversation conv)
		{
			addedConversation = conv;
		}
	}

	public class SuccessfullyLoggededEventArgs : EventArgs
	{
		IUser loggedUser;

		public IUser LoggedUser { get => loggedUser; set => loggedUser = value; }
		public SuccessfullyLoggededEventArgs(IUser user)
		{
			loggedUser = user;
		}
	}

	public class SuccessfullyRegisteredEventArgs : EventArgs
	{
		IUser registeredUser;

		public IUser RegisteredUser { get => registeredUser; set => registeredUser = value; }
		public SuccessfullyRegisteredEventArgs(IUser user)
		{
			registeredUser = user;
		}
	}
}