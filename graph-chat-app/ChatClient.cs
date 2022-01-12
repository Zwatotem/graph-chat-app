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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
		int responseLength = 0;
		bool goOn;

		public ClientChatSystem ChatSystem
		{
			get => chatSystem;
			set => chatSystem = value;
		}

		public Dispatcher Dispatcher
		{
			get => dispatcher;
			set => dispatcher = value;
		}

		public event EventHandler<SuccessfullyRegisteredEventArgs> SuccessfullyRegistered = (sender, e) => { };
		public event EventHandler<SuccessfullyLoggededEventArgs> SuccessfullyLogged = (sender, e) => { };
		public event EventHandler<EventArgs> UnSuccessfullyLogged = (sender, e) => { };

		public event EventHandler<SuccessfullyAddedConversationEventArgs> SuccessfullyAddedConversation =
			(sender, e) => { };


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
							bytesReceived += socket.Receive(headerBytes, bytesReceived, headerLength - bytesReceived,
								SocketFlags.None);
						}

						byte type = headerBytes[0];
						int messageLength = BitConverter.ToInt32(headerBytes, 1);
						bytesReceived = 0;
						while (bytesReceived < messageLength)
						{
							bytesReceived += socket.Receive(inBuffer, bytesReceived, messageLength - bytesReceived,
								SocketFlags.None);
						} //do poprawy, bufor moze byc za maly

						if (type == (byte)1)
						{
							Console.WriteLine("DEBUG: listener received boolean response");
							responseStatus = (inBuffer[0] == (byte)0) ? false : true;
							responseReady = true;
							responseLength = messageLength;
							Monitor.Pulse(this);
							Monitor.Wait(this);
						}
						else if (type == (byte)2)
						{
						}
						else if (type == (byte)3)
						{
							Console.WriteLine("DEBUG: listener received user to remove from conversation");
							Guid conversationId = new Guid(inBuffer[0..16]);
							Conversation conversation = ChatSystem.GetConversation(conversationId);
							string nameToRemove = Encoding.UTF8.GetString(inBuffer, 16, messageLength - 16);
							bool result = false;
							try
							{
								readWriteLock.AcquireWriterLock(lockTimeout);
								result = ChatSystem.LeaveConversation(nameToRemove, conversationId);
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
							Guid conversationId = new Guid(inBuffer[0..16]);
							Conversation conversation = ChatSystem.GetConversation(conversationId);
							string nameToAdd = Encoding.UTF8.GetString(inBuffer, 16, messageLength - 16);
							if (ChatSystem.GetUser(nameToAdd) == null)
							{
								try
								{
									readWriteLock.AcquireWriterLock(lockTimeout);
									ChatSystem.AddNewUser(nameToAdd);
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
								result = ChatSystem.AddUserToConversation(nameToAdd, conversationId);
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
							// Turns out it's also sent instead of serialized UserUpdates for some reason
							//Console.WriteLine("DEBUG: listener received serialized conversation");
							MemoryStream memStream = new MemoryStream(inBuffer, 0, messageLength);
							Conversation result;
							try
							{
								readWriteLock.AcquireWriterLock(lockTimeout);
								result = ChatSystem.AddConversation(memStream, new ConcreteDeserializer());
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
							var memStream1 = new MemoryStream(inBuffer);
							var memStream2 = new MemoryStream(inBuffer);
							var message = new Message(memStream1, new ConcreteDeserializer());
							Conversation conversation = ChatSystem.GetConversation(message.ConversationId);
							Message result = null;
							if (conversation != null)
							{
								this.Dispatcher.Invoke(() =>
									{
										try
										{
											readWriteLock.AcquireWriterLock(lockTimeout);
											result = conversation.AddMessage(memStream2, new ConcreteDeserializer());
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
						// Yay DO NOT ADD ANYTHING YET PEPEGA!!!!
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
						MemoryStream memStream = new MemoryStream(inBuffer, 1, responseLength - 1);
						IDeserializer deserializer = new ConcreteDeserializer();
						bool result = false;
						try
						{
							readWriteLock.AcquireWriterLock(lockTimeout);
							Console.WriteLine("DEBUG: listener received serialized user");
							User user = new User(memStream, deserializer);
							result = ChatSystem.logIn(user);
						}
						finally
						{
							readWriteLock.ReleaseWriterLock();
						}
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
				UnSuccessfullyLogged(this, new());
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
					var conv = ChatSystem.Conversations.Where(x => x.Value.Name == conversationName)
						.Select(x => x.Value).First();
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
					yourName = ChatSystem.LoggedUserName;
					if (yourName == null)
					{
						Console.WriteLine("You must be logged in first!");
						return false;
					}
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
					yourName = ChatSystem.LoggedUserName;
					if (yourName == null)
					{
						Console.WriteLine("You must be logged in first!");
						return false;
					}
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
			Guid conversationId = new Guid(Console.ReadLine());
			byte[] content = conversationId.ToByteArray();
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
						ChatSystem.LeaveConversation(yourName, conversationId);
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

		public async void requestSendMessage(object sender, SendMessageEventArgs args)
		{
			Console.WriteLine("DEBUG: attempt to {0}", "send message");
			Guid messageSenderId = args.MessageSender;
			Guid conversationId = args.ConversationID;
			Guid parentMessageId = args.ParentMessageID;
			IMessageContent text = args.MessageContent;
			var modelMessage = new Message(messageSenderId, parentMessageId, text, DateTime.Now)
				{ ConversationId = conversationId };
			modelMessage.ConversationId = conversationId;
			var netMessage = modelMessage.Serialize(new ConcreteSerializer()).ToArray();
			// {
			// 	var deserializedMessage = new Message(new MemoryStream(netMessage), new ConcreteDeserializer());
			// 	Assert.Equals(modelMessage.ID, deserializedMessage.ID);
			// 	Assert.Equals(modelMessage.ConversationId, deserializedMessage.ConversationId);
			// 	Assert.Equals(modelMessage.TargetId, deserializedMessage.TargetId);
			// 	Assert.Equals(modelMessage.AuthorID, deserializedMessage.AuthorID);
			// 	Assert.Equals(modelMessage.Content, deserializedMessage.Content);
			// }
			byte[] header = new byte[5];
			header[0] = 6;
			Array.Copy(BitConverter.GetBytes(netMessage.Length), 0, header, 1, 4);
			socket.Send(header);
			socket.Send(netMessage);
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
		public Guid MessageSender;
		public Guid ConversationID;
		public Guid ParentMessageID;
		public IMessageContent MessageContent;

		public SendMessageEventArgs(Guid user, Guid conv, IMessageContent content)
		{
			MessageSender = user;
			ConversationID = conv;
			ParentMessageID = Guid.Empty;
			MessageContent = content;
		}

		public SendMessageEventArgs(Guid user, Guid conv, Guid target, IMessageContent content)
			: this(user, conv, content)
		{
			ParentMessageID = target;
		}
	}

	public class SuccessfullyAddedConversationEventArgs
	{
		Conversation addedConversation;

		public Conversation AddedConversation
		{
			get => addedConversation;
			set => addedConversation = value;
		}

		public SuccessfullyAddedConversationEventArgs(Conversation conv)
		{
			addedConversation = conv;
		}
	}

	public class SuccessfullyLoggededEventArgs : EventArgs
	{
		IUser loggedUser;

		public IUser LoggedUser
		{
			get => loggedUser;
			set => loggedUser = value;
		}

		public SuccessfullyLoggededEventArgs(IUser user)
		{
			loggedUser = user;
		}
	}

	public class SuccessfullyRegisteredEventArgs : EventArgs
	{
		IUser registeredUser;

		public IUser RegisteredUser
		{
			get => registeredUser;
			set => registeredUser = value;
		}

		public SuccessfullyRegisteredEventArgs(IUser user)
		{
			registeredUser = user;
		}
	}
}