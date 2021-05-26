using ChatModel;

namespace GraphChatApp.ViewModel
{
	internal class ConversationCanvasViewModel : ViewModel
	{
		Conversation conversation;
		private string conversationName;

		public ConversationCanvasViewModel(Conversation conversation)
		{
			this.conversation = conversation;
			conversation.PropertyChanged += InvokePropertyChanged;
		}

		public string ConversationName { get => conversation.Name; }
	}
}