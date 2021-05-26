using System.IO;

namespace ChatModel.Util
{
    /// <summary>
    /// Interface representing binary serializer concept.
    /// </summary>
    public interface IDeserializer
    {
        /// <summary>
        /// Deserializes object from stream.
        /// </summary>
        /// <param name="stream">MemoryStream containing serialized object</param>
        /// <returns>Deserialized object.</returns>
        object deserialize(Stream stream);
    }
}

/*
Compliant with Interface segregation, as it has only one purpose and one method. Interface structure encourages extension over modification of code.
Allows for other objects to reference it rather that a concrete class and thus enables dependency inversion.
*/