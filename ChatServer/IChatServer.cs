
namespace ChatServer
{
    public interface IChatServer
    {
        void startServer();

        bool isWorking();

        void shutdown();
    }
}
