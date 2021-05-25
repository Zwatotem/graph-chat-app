using System;﻿
using System.Text;

namespace ChatModel
{
	/// <summary>
	/// Implementation of IMessageContent representing text content.
	/// </summary>
	[Serializable]
	public class TextContent : IMessageContent
	{
		private string dataString; //field storing text data

		public TextContent(string dataString)
		{
			this.dataString = dataString;
		}

		public object getData()
		{
			return dataString;
		}

		public byte[] serialize()
		{
			return Encoding.UTF8.GetBytes(dataString); //converts string to byte[] using UTF-8 encoding
		}
	}
}

/*
Compliant with Liskov Substitution as it properly implements all interface methods. Doesn't add any new methods, so it still 
has only a single responsibility.
*/