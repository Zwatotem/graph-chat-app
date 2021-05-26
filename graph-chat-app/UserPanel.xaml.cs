using ChatModel;
using GraphChatApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for UserPanel.xaml
	/// </summary>
	public partial class UserPanel : Page
	{
		public string chatModelNamespace;
		public MainWindow window;
		public ConversationCreationPage conversationCreationPage;
		Dictionary<Conversation, ConversationCanvasPage> conversationCanvasPages;
		ConversationCollectionViewModel ViewModel;
		public UserPanel(MainWindow window)
		{
			InitializeComponent();
			this.window = window;
			conversationCanvasPages = new Dictionary<Conversation, ConversationCanvasPage>();
			ViewModel = new ConversationCollectionViewModel(window.app.Client.ChatSystem, EnterConversationCanvas);
			DataContext = ViewModel;
			conversationCreationPage = new ConversationCreationPage(window);
		}

		private void AddConversation(object sender, RoutedEventArgs e)
		{
			window.MainFrame.NavigationService.Navigate(conversationCreationPage);
		}

		private void EnterConversationCanvas(Conversation conversation)
		{
			if (!conversationCanvasPages.ContainsKey(conversation))
			{
				conversationCanvasPages.Add(conversation, new ConversationCanvasPage(window, conversation));
			}
			window.MainFrame.NavigationService.Navigate(conversationCanvasPages[conversation]);
		}
	}
}
