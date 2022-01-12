using ChatModel;

namespace GraphChatApp.ViewModel;

public class TextContentViewerViewModel : ChatModel.Util.ViewModel
{
	public TextContentViewerViewModel(TextContent textContent)
	{
		this.TextContent = textContent;
	}

	public TextContent TextContent { get; set; }

	public string TextData { get=>TextContent.TextData; set=>TextContent.TextData=value; }
}