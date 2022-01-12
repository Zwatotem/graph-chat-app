using System;
using System.Text;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Implementation of IMessageContent representing text content.
/// </summary>
[Serializable]
public class TextContent : IMessageContent
{
	private string dataString; //field storing text data
	public TextContent()
	{
		dataString = String.Empty;
	}

	public TextContent(string dataString)
	{
		this.dataString = dataString;
	}

	public string TextData
	{
		get => dataString;
		set => dataString = value;
	}

	public string getData()
	{
		return dataString;
	}
	[field: NonSerialized]
	public IContentViewModelProvider ContentViewModelProvider { get; set; }
}

/*
Compliant with Liskov Substitution as it properly implements all interface methods. Doesn't add any new methods, so it still 
has only a single responsibility.
*/