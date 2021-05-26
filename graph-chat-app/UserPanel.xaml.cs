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
		public UserPanel(MainWindow window)
		{
			InitializeComponent();
			this.window = window;
			DataContext = window.app.Client.ChatSystem;
			conversationCreationPage = new ConversationCreationPage(window);
		}

		private void AddConversation(object sender, RoutedEventArgs e)
		{
			window.MainFrame.NavigationService.Navigate(conversationCreationPage);
		}
	}
}
