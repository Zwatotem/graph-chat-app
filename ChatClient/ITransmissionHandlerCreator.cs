
namespace ChatClient
{
    public interface ITransmissionHandlerCreator
    {
        ITransmissionHandler createTransmissionHandler(int type);
    }
}
