
namespace ChatModel
{
    /// <summary>
    /// Interface containing factory method for creating IMessageContent objects.
    /// </summary>
    public interface IMessageContentCreator
    {
        /// <summary>
        /// Factory method creating IMessageContent object.
        /// </summary>
        /// <param name="data">Byte array containing data from which content is to be created</param>
        /// <param name="offset">Number of bytes to skip at the beginning of array</param>
        /// <returns>Instance of class implementing IMessageContent.</returns>
        IMessageContent createMessageContent(byte[] data, int offset);
    }
}

/*
Model example of compliance with SOLID: one responsibility (even only one method), interface has only one method, 
interface encourages extension and enables dependency inversion. It is the abstract part of the factory method design pattern.
*/