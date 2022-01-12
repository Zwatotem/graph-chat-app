using System;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer
{
	/// <summary>
	/// Concrete implementation of IClientHandler.
	/// </summary>
	public class ConcreteClientHandler : IClientHandler
	{
		private IIOSocketFacade socketFacade; //socket facade through which communication with client is conducted
		private string handledUserName;

		private IServerChatSystem
			chatSystem; //chatSystem storing and modifying all information about conversations, messages and so forth

		private List<IClientHandler> allHandlers; //list of all handlers handling connected clients
		private Thread listenerThread; //thread that is to listen for client requests
		private bool working;

		private IRequestHandlerCreator
			requestHandlerCreator; //contains the factory method necessary for creating instances of IRequestHandler

		private int requestHeaderLength; //length of request's header - may vary depending on the implementation

		public string HandledUserName
		{
			get => handledUserName;
			set => handledUserName = value;
		}

		public bool Working
		{
			get => working;
		}

		public ConcreteClientHandler(IServerChatSystem chatSystem, Socket socket, List<IClientHandler> allHandlers,
			int headerLength)
		{
			Console.WriteLine("DEBUG: handler object created");
			this.chatSystem = chatSystem;
			this.socketFacade =
				new ConcreteIOSocketFacade(socket); //socket is enclosed withing the facade as to simplify its usage
			this.allHandlers = allHandlers;
			this.requestHandlerCreator = new ConcreteRequestHandlerCreator();
			this.requestHeaderLength = headerLength;
			this.listenerThread = new Thread(this.listen); //listener thread will be executing listen method
			this.working = false;
			this.handledUserName = null;
		}

		public void startWorking()
		{
			if (!working) //only start working if it isn't working at the moment
			{
				working = true;
				listenerThread.Start();
			}
		}

		public void sendMessage(byte typeByte, byte[] message)
		{
			socketFacade.sendMessage(typeByte, message); //use the socket facade to send a message to client
		}

		public void shutdown()
		{
			working = false;
			lock (allHandlers) //prohibit other modifications as the handler stops working 
			{
				allHandlers.Remove(this); //remove handler from the server's list of active handlers
			}

			socketFacade.shutdown();
		}

		/// <summary>
		/// Listens for requests from the client and responds accordingly.
		/// </summary>
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
					typeByte = headerBytes[0]; //type byte is the firt one in header
					int messageLength =
						BitConverter.ToInt32(headerBytes, 1); //decode how long the message to receive is
					messageBytes = socketFacade.receiveMessage(messageLength);
				}
				catch (SocketException ex)
				{
					Console.WriteLine("DEBUG: SocketException thrown: {0}", ex.Message);
					if(ex.SocketErrorCode==SocketError.ConnectionAborted)
						shutdown();
					typeByte = 0; //if exceptions occured the handler is to shutdown, setting type byte to allow for that
				}

				IRequestHandler
					requestHandler =
						requestHandlerCreator.createRequestHandler(typeByte); //using factory method to create request
				//handler. Usage of this design pattern is highly beneficial, as this code is not concerned with what type o handler is necessary.

				requestHandler.handleRequest(allHandlers, chatSystem, this,
					messageBytes); //using strategy pattern to handle client request.
				//This pattern is useful, as we are not concerned with what request have been received. Rather as we received from the factory
				//method a request handler with the correct instance implementing IHandleStrategy, we can simple call a universal handling method.
			}
		}
	}
}

/*
1. Single resposibility - it only has logic for handling interactions with client.
2. Open/closed - rather than changing this class, one can just implement the interface anew.
3. Liskov Substitution - all methods of interface (the contract) are properly implemented.
5. Dependency inversion - references within the class (eg. to request handler) are to to interface types, not concrete classes.
*/