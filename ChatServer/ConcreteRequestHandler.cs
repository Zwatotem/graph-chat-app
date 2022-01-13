using System.Collections.Generic;
using ChatModel;

namespace ChatServer;

/// <summary>
/// Class representing handler of client's requests.
/// </summary>
public class ConcreteRequestHandler : IRequestHandler
{
	private IHandleStrategy handleStrategy; //currently used handling strategy
	public ConcreteRequestHandler(IHandleStrategy handleStrategy) //conctructs new instance with given strategy
	{
		this.handleStrategy = handleStrategy;
	}
	public IHandleStrategy HandleStrategy
	{
		get => handleStrategy;
		set => handleStrategy = value;
	}
	public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem,
		IClientHandler handlerThread, byte[] messageBytes)
	{
		handleStrategy.handleRequest(allHandlers, chatSystem, handlerThread, messageBytes);
	}
}

/*
This class is the concrete contex of the implemented strategy pattern. It has one resposibility, implements all methods of the interface
(Liskov Sustitution). Thanks to the strategy pattern, this class does not have to know what request it is handling exactly, rather it knows 
only, that it is to use the strategy thats is currently referenced in handleStrategy field (doesn't matter what concrete instatnce it references).
*/