
namespace ChatServer
{
    public interface IRequestHandlerCreator
    {
        IRequestHandler createRequestHandler(byte typeByte);
    }
}
