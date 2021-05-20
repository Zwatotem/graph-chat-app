using System;﻿
using System.Text;

namespace ChatModel
{
	[Serializable]
	public class TextContent : IMessageContent //respresents text content of a message
	{
		private string dataString; //field storing text data

		/// <summary>
		/// Creates an immutable TextContent object, initialized with text passed as parameter.
		/// </summary>
		/// <param name="dataString"></param>
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
