using ChatServer.HandleStrategies;

namespace ChatServer;

/// <summary>
/// Concrete implementation of IRequestHandlerCreator.
/// </summary>
class ConcreteRequestHandlerCreator : IRequestHandlerCreator
{
	public IRequestHandler createRequestHandler(byte typeByte)
	{
		IRequestHandler createdRequestHandler = null;
		switch
			(typeByte) //choose a strategy to supply to the strategy pattern context (IRequestHandler) depending on the type byte
		{
			case 0:
				createdRequestHandler = new ConcreteRequestHandler(new HandleDisconnectStrategy());
				break;
			case 1:
				createdRequestHandler = new ConcreteRequestHandler(new HandleNewUserStrategy());
				break;
			case 2:
				createdRequestHandler = new ConcreteRequestHandler(new HandleLoginStrategy());
				break;
			case 3:
				createdRequestHandler = new ConcreteRequestHandler(new HandleNewConversationStrategy());
				break;
			case 4:
				createdRequestHandler = new ConcreteRequestHandler(new HandleAddToConversationStrategy());
				break;
			case 5:
				createdRequestHandler = new ConcreteRequestHandler(new HandleLeaveConversationStrategy());
				break;
			case 6:
				createdRequestHandler = new ConcreteRequestHandler(new HandleSendMessageStrategy());
				break;
			case 7:
				createdRequestHandler = new ConcreteRequestHandler(new HandleRequestUserStrategy());
				break;
			default:
				createdRequestHandler = new ConcreteRequestHandler(new HandleUnknownStrategy());
				break;
		}
		return createdRequestHandler; //return the created instance referenced as interface type
	}
}

/*
This class is the concrete part of the factory design pattern as it contains the factory method creating instances of IRequestHandler.
It's compliant with Liskov Substitution as it properly implements all of interface's methods (just one in this case).
*/