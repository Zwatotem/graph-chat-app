using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatModel;

namespace chatAppTest //test comment
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