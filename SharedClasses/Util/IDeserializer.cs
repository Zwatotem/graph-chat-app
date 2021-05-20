using System.IO;

namespace ChatModel.Util
{
    public interface IDeserializer
    {
        object deserialize(Stream stream);
    }
}
