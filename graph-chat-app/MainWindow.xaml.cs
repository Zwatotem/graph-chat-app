using System;
using System.Windows;
using GraphChatApp;

namespace GraphChatApp
{
	public class UserRegisteredEventArgs : EventArgs
	{
		string username;
		public UserRegisteredEventArgs(string username)
		{
			this.username = username;
		}

		public string Username { get => username; set => username = value; }
	}


	public class UserLoggedEventArgs
	{
		string username;
		public UserLoggedEventArgs(string username)
		{
			this.username = username;
		}

		public string Username { get => username; set => username = value; }
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		App app;
		WelcomeScreen welcomeScreen;
		RegistrationPage registrationPage;


		LogInPage logInPage;

		public event EventHandler<UserRegisteredEventArgs> UserRegistered;
		public event EventHandler<UserLoggedEventArgs> UserLogged;

		public MainWindow()
		{
			InitializeComponent();
			app = (App)System.Windows.Application.Current;
			welcomeScreen = new WelcomeScreen(this);
			registrationPage = new RegistrationPage(this);
			logInPage = new LogInPage(this);

			MainFrame.NavigationService.Navigate(welcomeScreen);
		}

		internal void OpenWelcomeScreen()
		{
			while(MainFrame.NavigationService.CanGoBack)
			{
				MainFrame.NavigationService.GoBack();
			}
		}

		internal void OpenLoginPage()
		{
			MainFrame.NavigationService.Navigate(logInPage);
		}

		internal void OpenRegistrationPage()
		{
			MainFrame.NavigationService.Navigate(registrationPage);
			app.Client.SuccessfullyRegistered += (object s, SuccessfullyRegisteredEventArgs a) => OpenWelcomeScreen(); ;
		}

		internal void OnUserRegistered(string username)
		{
			UserRegistered.Invoke(this, new(username));
		}

		internal void OnUserLogged(string username)
		{
			UserLogged.Invoke(this, new(username));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			app.InitializeWithGUI();
		}
	}

}
