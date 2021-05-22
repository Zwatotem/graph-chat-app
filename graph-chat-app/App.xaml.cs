using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		ChatClient client;
		MainWindow mainWindow;
		App()
		{
			Client = new ChatClient("192.168.1.13", 50000);
			Client.workClient();
		}

		public ChatClient Client { get => client; set => client = value; }

		public void InitializeWithGUI()
		{
			mainWindow = (MainWindow)MainWindow;
			mainWindow.UserRegistered += Client.requestCreateNewUser;
			mainWindow.UserLogged += Client.requestLogIn;
		}
	}
}
