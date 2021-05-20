using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util
{
    public class ConcreteDeserializer : IDeserializer
    {
        public object deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
}
