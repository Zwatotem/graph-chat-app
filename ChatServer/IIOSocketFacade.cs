
namespace ChatServer
{
    public interface IIOSocketFacade
    {
        byte[] receiveMessage(int length);

        void sendMessage(byte typeByte, byte[] message);

        void shutdown();
    }
}
