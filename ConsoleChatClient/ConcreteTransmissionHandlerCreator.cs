using ChatClient.HandleTransmissionStrategies;

namespace ChatClient
{
    /// <summary>
    /// Concrete implementation of ITransmissionHandlerCreator.
    /// </summary>
    public class ConcreteTransmissionHandlerCreator : ITransmissionHandlerCreator
    {
        public ITransmissionHandler createTransmissionHandler(int type)
        {
            ITransmissionHandler createdHandler = null;
            switch(type)
            {
                case 3:
                    createdHandler = new ConcreteTransmissionHandler(new HandleRemoveUserTransmissionStrategy());
                    break;
                case 4:
                    createdHandler = new ConcreteTransmissionHandler(new HandleAddUserTransmissionStrategy());
                    break;
                case 5:
                    createdHandler = new ConcreteTransmissionHandler(new HandleConversationTransmissionStrategy());
                    break;
                case 6:
                    createdHandler = new ConcreteTransmissionHandler(new HandleMessageTransmissionStrategy());
                    break;
                default:
                    createdHandler = null;
                    break;
            }
            return createdHandler;
        }
    }
}

/*
Concrete part of factory method pattern.
Analogus in compliance with solid to factory method pattern in ChatServer.
*/