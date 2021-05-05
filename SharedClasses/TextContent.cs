
namespace ChatModel
{
	public class TextContent : MessageContent //class representing text content of a message, where text is stored in a string
													  //objects of this class are immutable
	{
		private string dataString; //field storing the data

		public TextContent(string dataString) //simple constructor
		{
			this.dataString = dataString;
		}

		public object getData() //returns the text of a message
		{
			return dataString;
		}

		public object serialize() //serializes the object
		{
			return dataString;
		}
	}
}
