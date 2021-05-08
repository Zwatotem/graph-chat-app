using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chatAppTest
{
	[TestClass]
	public class TextContentTest
	{
		[TestMethod]
		public void getDataTest()
		{
			TextContent content = new TextContent("Alamakota");
			Assert.AreSame(content.getData(), "Alamakota");
		}
	}
}