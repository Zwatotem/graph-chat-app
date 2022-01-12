using ChatModel;

namespace GraphChatApp.ViewModel;

public class WPFContentViewModelProvider : IContentViewModelProvider
{
	public ChatModel.Util.ViewModel? getCompositorViewModel(IMessageContent content)
	{
		switch (content)
		{
			case TextContent textContent:
				return new TextContentCompositorViewModel(textContent);
			default:
				return null;
		}
	}

	public ChatModel.Util.ViewModel? getViewerViewModel(IMessageContent content)
	{
		switch (content)
		{
			case TextContent textContent:
				return new TextContentViewerViewModel(textContent);
			default:
				return null;
		}
	}
}