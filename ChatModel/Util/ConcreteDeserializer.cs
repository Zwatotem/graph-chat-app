using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util;

/// <summary>
/// Concrete implementation of IDeserializer.
/// </summary>
public class ConcreteDeserializer : IDeserializer
{
	public object Deserialize(Stream stream)
	{
		BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
		return formatter.Deserialize(stream); //uses a BinaryFormatter to Deserialize
#pragma warning restore SYSLIB0011 // Type or member is obsolete
	}
}

/*
Complies with Liskov Substitution principles, as it properly implements all interface methods (here just one).
Has only a single resposibility.
*/