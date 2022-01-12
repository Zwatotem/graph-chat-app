using ChatModel;
using GraphChatApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
			this.window = window;
			this.conversation = conversation;
			viewModel = new ConversationCanvasViewModel(conversation, App.Current.ChatSystem);
			DataContext = viewModel;
		}
	}
}
