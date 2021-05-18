using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class ConcreteRequestHandlerCreator : IRequestHandlerCreator
    {
        public IRequestHandler createRequestHandler(byte typeByte)
        {
            IRequestHandler createdRequestHandler = null;
            switch(typeByte)
            {
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
                    createdRequestHandler = new ConcreteRequestHandler(new HandleUnknownRequest());
                    break;
            }
            return createdRequestHandler;
        }
    }
}
