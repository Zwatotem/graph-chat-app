using System.Text;

namespace ChatModel
{
    /// <summary>
    /// Concrete implementation of IMessageContentCreator.
    /// </summary>
    public class ConcreteMessageContentCreator : IMessageContentCreator
    {
        public IMessageContent createMessageContent(byte[] data, int offset)
        {
            //first byte of proper data indicates the content's type
            if (data[0 + offset] == 1) //text message
            {
                return new TextContent(Encoding.UTF8.GetString(data, 1 + offset, data.Length - 1 - offset)); //decode text and instantiate object
            }
            else //unrecognized type of message
            {
                return null;
            }
        }
    }
}

/*
Class has only a single resposibility and complies with Liskov Substitution as the only interface (contract) method is properly implemented. 
It is the concrete factory class in the factory method design pattern.
*/