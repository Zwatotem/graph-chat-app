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
		private ConversationCollectionViewModel viewModel;
		public UserPanel(MainWindow window)
		{
			InitializeComponent();
			this.window = window;
			//viewModel = new ConversationCollectionViewModel(window.app.Client.ChatSystem);
			DataContext = window.app.Client.ChatSystem;
		}

		private void AddConversation(object sender, RoutedEventArgs e)
		{
			window.OnConversationAdded(DateTime.Now.ToString());
		}

		internal void displayNewConversation(object sender, SuccessfullyAddedConversationEventArgs e)
		{
		}
	}
}
