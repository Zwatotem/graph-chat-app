using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util
{
	/// <summary>
	/// Concrete instance of ISerializer.
	/// </summary>
    public class ConcreteSerializer : ISerializer
    {
		public MemoryStream Serialize(object arg)
		{
			MemoryStream stream = new MemoryStream(); //creates MemoryStream
			var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
			formatter.Serialize(stream, arg); //uses BinaryFormatter to Serialize the object into the stream
#pragma warning restore SYSLIB0011 // Type or member is obsolete
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