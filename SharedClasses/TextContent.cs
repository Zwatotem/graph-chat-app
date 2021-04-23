using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
	public class TextContent : MessageContent
	{
		private string v;

		public TextContent(string v)
		{
			this.v = v;
		}

		public object getData()
		{
			throw new NotImplementedException();
		}
	}
}
