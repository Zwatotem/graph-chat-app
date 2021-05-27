using ChatClient.HandlePanelStrategies;

namespace ChatClient
{
    public class ConcretePanelHandlerCreator : IPanelHandlerCreator
    {
        public IPanelHandler createPanelHandler(int type)
        {
            IPanelHandler createdHandler;
            switch(type)
            {
                case 0:
                    createdHandler = new ConcretePanelHandler(new HandleQuitPanelStrategy());
                    break;
                case 10:
                    createdHandler = new ConcretePanelHandler(new HandleWelcomePanelStrategy());
                    break;
                case 1001:
                    createdHandler = new ConcretePanelHandler(new HandleRegisterPanelStrategy());
                    break;
                case 1002:
                    createdHandler = new ConcretePanelHandler(new HandleLogInPanelStrategy());
                    break;
                case 20:
                    createdHandler = new ConcretePanelHandler(new HandleUserPanelStrategy());
                    break;
                case 2001:
                    createdHandler = new ConcretePanelHandler(new HandleNewConversationPanelStrategy());
                    break;
                case 2002:
                    createdHandler = new ConcretePanelHandler(new HandleLeaveConversationPanelStrategy());
                    break;
                case 2003:
                    createdHandler = new ConcretePanelHandler(new HandleChooseConversationPanelStrategy());
                    break;
                case 30:
                    createdHandler = new ConcretePanelHandler(new HandleDisplayConversationPanelStrategy());
                    break;
                case 3001:
                    createdHandler = new ConcretePanelHandler(new HandleAddUserPanelStrategy());
                    break;
                case 3002:
                    createdHandler = new ConcretePanelHandler(new HandleSendMessagePanelStrategy());
                    break;
                case 3003:
                    createdHandler = new ConcretePanelHandler(new HandleUsersListPanelStrategy());
                    break;
                default:
                    createdHandler = null;
                    break;
            }
            return createdHandler;
        }
    }
}
