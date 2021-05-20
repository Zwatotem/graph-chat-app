using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatModel.Util
{
    public class ConcreteSerializer : ISerializer
    {
		public MemoryStream serialize(object arg)
		{
			MemoryStream stream = new MemoryStream();
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, arg);
			stream.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
