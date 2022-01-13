using ChatModel.Util;

namespace ChatModel;

public interface IContentViewModelProvider
{
	public ViewModel? getCompositorViewModel(IMessageContent content);
	public ViewModel? getViewerViewModel(IMessageContent content);
}