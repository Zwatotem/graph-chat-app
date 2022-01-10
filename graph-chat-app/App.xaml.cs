using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		
		public static App Current
		{
			get;
			private set;
		}
		
		ChatClient client;
		ClientChatSystem chatSystem;
		MainWindow mainWindow;
		App() : base()
		{
			Current = this;
			Client = new ChatClient("127.0.0.1", 50000, this.Dispatcher);
			chatSystem = Client.ChatSystem;
			Client.workClient();
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => Unload();
		}

		public ChatClient Client { get => client; set => client = value; }
		public ClientChatSystem ChatSystem { get => chatSystem; }

		public void InitializeWithGUI()
		{
			mainWindow = (MainWindow)MainWindow;
			mainWindow.UserRegistered += Client.requestCreateNewUser;
			mainWindow.UserLogged += Client.requestLogIn;
			mainWindow.ConversationAdded += Client.requestAddConversation;
		}

		internal void Unload()
		{
			Client.requestDisconnect();
		}
	}
}
