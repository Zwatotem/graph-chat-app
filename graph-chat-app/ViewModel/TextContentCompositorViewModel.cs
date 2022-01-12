using ChatModel;

namespace GraphChatApp.ViewModel;

public class TextContentCompositorViewModel : ChatModel.Util.ViewModel
{
	public TextContentCompositorViewModel(TextContent textContent)
	{
		this.TextContent = textContent;
	}

	public TextContent TextContent { get; set; }

	public string TextData
	{
		get=>TextContent.TextData;
		set
		{
			TextContent.TextData = value;
			OnPropertyChanged(this, new(nameof(TextData)));
		}
	}
}