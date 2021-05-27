using System;
using ChatClient.HandleTransmissionStrategies;

namespace ChatClient
{
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
