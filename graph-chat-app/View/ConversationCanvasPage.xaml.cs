using ChatModel;
using GraphChatApp.ViewModel;
using System.Windows;
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
			this.window = window;
			this.conversation = conversation;
			viewModel = new ConversationCanvasViewModel(conversation);
			DataContext = viewModel;
		}
		
		private void SendMessage(object sender, RoutedEventArgs e)
		{
			var currentUser = window.app.Client.ChatSystem.getUser(window.app.Client.ChatSystem.getUserName());
			window.app.Client.requestSendTextMessage(this, new(currentUser, conversation.ID, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris at pharetra massa, nec ultrices tortor."));
		}
	}
}
