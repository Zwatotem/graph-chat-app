
namespace ChatModel
{
    public interface IMessageContentCreator
    {
        IMessageContent createMessageContent(byte[] data, int offset);
    }
}
