using ChatModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using GraphChatApp.ViewModel;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static App Current { get; private set; }

		ChatClient client;
		ClientChatSystem chatSystem;
		MainWindow mainWindow;

		App() : base()
		{
			Current = this;
			Client = new ChatClient("127.0.0.1", 50000, this.Dispatcher);
			chatSystem = Client.ChatSystem;
			ContentViewModelProvider = new WPFContentViewModelProvider();
			Client.workClient();
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => Unload();
		}

		public ChatClient Client
		{
			get => client;
			set => client = value;
		}

		public ClientChatSystem ChatSystem
		{
			get => chatSystem;
		}

		public IContentViewModelProvider ContentViewModelProvider { get; set; }

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
/*
 * App class is a singleton. It has only one private constructor and the only way to instantiate it is through static
 * attribute Current. That way we're sure there is only ever one App class in the entire application. Other classes aren't
 * encapsulated as singletons, instead they are attached to the App class, so that they can be used in multiplicity
 * for testing, or if the case requires it.
 */