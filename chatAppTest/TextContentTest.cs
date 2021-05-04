using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatModel;

namespace chatAppTest
{
	[TestClass]
	public class TextContentTest
	{
		[TestMethod]
		public void getDataTest()
		{
			TextContent content = new TextContent("Alamakota");
			Assert.AreSame(content.getData(),"Alamakota");
		}
	}
}