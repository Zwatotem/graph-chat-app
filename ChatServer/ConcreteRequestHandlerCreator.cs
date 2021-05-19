using ChatServer.HandleStrategies;

namespace ChatServer
{
    class ConcreteRequestHandlerCreator : IRequestHandlerCreator
    {
        public IRequestHandler createRequestHandler(byte typeByte)
        {
            IRequestHandler createdRequestHandler = null;
            switch(typeByte)
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
                default:
                    createdRequestHandler = new ConcreteRequestHandler(new HandleUnknownStrategy());
                    break;
            }
            return createdRequestHandler;
        }
    }
}
