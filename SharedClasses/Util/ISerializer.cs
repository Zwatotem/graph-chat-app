using System.IO;

namespace ChatModel.Util
{
    public interface ISerializer
    {
        MemoryStream serialize(object arg);
    }
}
