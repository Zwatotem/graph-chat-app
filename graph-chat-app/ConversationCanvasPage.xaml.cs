using ChatModel;
using GraphChatApp.ViewModel;
using System.Windows.Controls;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for ConversationCanvasPage.xaml
	/// </summary>
	public partial class ConversationCanvasPage : Page
	{
		MainWindow window;
		ConversationCanvasViewModel viewModel;
		Conversation conversation;
		public ConversationCanvasPage(MainWindow window, Conversation conversation)
		{
			InitializeComponent();
			this.conversation = conversation;
			viewModel = new ConversationCanvasViewModel(conversation);
			DataContext = viewModel;
			ConversationTitle
		}
	}
}
