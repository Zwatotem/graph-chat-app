using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatModel;

namespace chatAppTest
{
	[TestClass]
	public class TextContentTest
	{
		[TestMethod]
		void GetDataTest()
		{
			TextContent content = new TextContent("Alamakota");
			Assert.AreSame(content.getData(),"Alamakota");
		}
	}
}