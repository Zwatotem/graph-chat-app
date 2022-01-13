using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GraphChatApp;

namespace GraphChatApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public App app;
	WelcomeScreen welcomeScreen;
	RegistrationPage registrationPage;
	LogInPage logInPage;
	UserPanel userPanel;

	public event EventHandler<UserRegisteredEventArgs> UserRegistered;
	public event EventHandler<UserLoggedEventArgs> UserLogged;
	public event EventHandler<ConversationAddedEventArgs> ConversationAdded;

	public MainWindow()
	{
		InitializeComponent();
		app = App.Current;
		welcomeScreen = new WelcomeScreen(this);
		registrationPage = new RegistrationPage(this);
		logInPage = new LogInPage(this);
		userPanel = new UserPanel(this);

		MainFrame.NavigationService.Navigate(welcomeScreen);
	}

	internal void OpenWelcomeScreen()
	{
		app.Client.SuccessfullyRegistered -= (object s, SuccessfullyRegisteredEventArgs a) => OpenWelcomeScreen();
		while (MainFrame.NavigationService.CanGoBack)
		{
			MainFrame.NavigationService.GoBack();
		}

	}

	internal void DoOpenUserPanel(object s, SuccessfullyLoggededEventArgs a)
	{
		OpenUserPanel();
	}
	
	internal void OpenLoginPage()
	{
		MainFrame.NavigationService.Navigate(logInPage);
		app.Client.SuccessfullyLogged += DoOpenUserPanel;
		app.Client.UnSuccessfullyLogged += DoShowFailMonit;
		app.Client.SuccessfullyRemovedConversation += DoCleanViewModel;
	}

	private void DoCleanViewModel(object? sender, SuccessfullyRemovedConversationEventArgs e)
	{
		CleanViewModel(e.ConversationID);
	}

	private void CleanViewModel(Guid eConversationId)
	{
		var nav = MainFrame;
		do
		{
			nav.RemoveBackEntry();
		}
		while (nav.CanGoBack);
	}

	private void DoShowFailMonit(object? sender, EventArgs e)
	{
		ShowFailMonit();
	}

	private void ShowFailMonit()
	{
		this.logInPage.ShowBadUserError();
	}

	private void OpenUserPanel()
	{
		app.Client.SuccessfullyLogged -= DoOpenUserPanel;
		app.Client.UnSuccessfullyLogged -= DoShowFailMonit;
		MainFrame.NavigationService.Navigated += ClearNavigationHistory;
		MainFrame.NavigationService.Navigate(userPanel);
		registrationPage = null;
		logInPage = null;
		welcomeScreen = null;
	}

	private void ClearNavigationHistory(object sender, NavigationEventArgs e)
	{
		Frame nav = sender as Frame;
		do
		{
			nav.RemoveBackEntry();
		}
		while (nav.CanGoBack);
		nav.Navigated -= ClearNavigationHistory;
	}

	internal void OpenRegistrationPage()
	{
		MainFrame.NavigationService.Navigate(registrationPage);
		app.Client.SuccessfullyRegistered += (object s, SuccessfullyRegisteredEventArgs a) => OpenWelcomeScreen();
	}

	internal void OnUserRegistered(string username)
	{
		UserRegistered.Invoke(this, new(username));
	}

	internal void OnUserLogged(string username)
	{
		UserLogged.Invoke(this, new(username));
	}

	internal void OnConversationAdded(string conversationName, string[] usernames)
	{
		ConversationAdded.Invoke(this, new(conversationName, usernames));
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		app.InitializeWithGUI();
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		app.Unload();
	}
}

public class ConversationAddedEventArgs : EventArgs
{
	string conversationName;
	string[] users;
	public ConversationAddedEventArgs(string conversationName, params string[] usernames)
	{
		this.conversationName = conversationName;
		Users = usernames;
	}

	public string ConversationName { get => conversationName; set => conversationName = value; }
	public string[] Users { get => users; set => users = value; }
}

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