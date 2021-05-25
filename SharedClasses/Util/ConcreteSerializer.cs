using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util
{
	/// <summary>
	/// Concrete instance of ISerializer.
	/// </summary>
    public class ConcreteSerializer : ISerializer
    {
		public MemoryStream serialize(object arg)
		{
			MemoryStream stream = new MemoryStream(); //creates MemoryStream
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, arg); //uses BinaryFormatter to serialize the object into the stream
			stream.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}

/*
Complies with Liskov Substitution principles, as it properly implements all interface methods (here just one).
Has only a single resposibility.
*/