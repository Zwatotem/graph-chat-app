using System.Text;

namespace ChatModel
{
    public class ConcreteMessageContentCreator : IMessageContentCreator
    {
        public IMessageContent createMessageContent(byte[] data, int offset)
        {
            if (data[0 + offset] == 1) //text message
            {
                return new TextContent(Encoding.UTF8.GetString(data, 1 + offset, data.Length - 1 - offset));
            }
            else //unrecognized type of message
            {
                return null;
            }
        }
    }
}
