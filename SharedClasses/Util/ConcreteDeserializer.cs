using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util
{
    /// <summary>
    /// Concrete implementation of IDeserializer.
    /// </summary>
    public class ConcreteDeserializer : IDeserializer
    {
        public object deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream); //uses a BinaryFormatter to deserialize
        }
    }
}

/*
Complies with Liskov Substitution principles, as it properly implements all interface methods (here just one).
Has only a single resposibility.
*/