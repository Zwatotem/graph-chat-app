using System.IO;

namespace ChatModel.Util;

/// <summary>
/// Interface representing binary serializer concept.
/// </summary>
public interface ISerializer
{
	/// <summary>
	/// Serializes the object.
	/// </summary>
	/// <param name="arg">Object to Serialize</param>
	/// <returns>MemoryStream of serialized object bytes.</returns>
	MemoryStream Serialize(object arg);
}

/*
Compliant with Interface segregation, as it has only one purpose and one method. Interface structure encourages extension over modification of code.
Allows for other objects to reference it rather that a concrete class and thus enables dependency inversion.
*/