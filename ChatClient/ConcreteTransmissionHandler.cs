
namespace ChatClient
{
    public class ConcreteTransmissionHandler : ITransmissionHandler
    {
        private IHandleTransmissionStrategy handleStrategy;

        public ConcreteTransmissionHandler(IHandleTransmissionStrategy handleStrategy)
        {
            this.handleStrategy = handleStrategy;
        }

        public void handle(ChatClient client, byte[] inBuffer)
        {
            handleStrategy.handle(client, inBuffer);
        }
    }
}
