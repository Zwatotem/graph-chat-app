using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GraphChatApp.ViewModel;

class CreateConversationCommand : ICommand
{
	public event EventHandler CanExecuteChanged = (sender, e) => { };
	private Func<bool> canExecute;
	private Action execute;
	public CreateConversationCommand(Func<bool> canExecute, Action execute)
	{
		this.canExecute = canExecute;
		this.execute = execute;
	}

	public void InvokeCanExecuteChanged()
	{
		CanExecuteChanged(this, new());
	}

	public bool CanExecute(object parameter)
	{
		return canExecute();
	}

	public void Execute(object parameter)
	{
		execute();
	}
}
class ConversationCreatorViewModel : ChatModel.Util.ViewModel
{
	MainWindow window;

	string csvusers;
	string conversationName;

	/// <summary>
	/// A user-inputed string representing comma-separated usernames
	/// </summary>
	public string CSVUsers
	{
		get
		{
			return csvusers;
		}
		set
		{
			csvusers = value;
			CreateConversation.InvokeCanExecuteChanged();
		}
	}

	/// <summary>
	/// Proposed name of conversation to be added
	/// </summary>
	public string ConversationName
	{
		get
		{
			return conversationName;
		}
		set
		{
			conversationName = value;
			CreateConversation.InvokeCanExecuteChanged();
		}
	}

	/// <summary>
	/// Names of users to be added to new conversation
	/// </summary>
	List<string> usernames
	{
		get
		{
			var currentUser = window.app.Client.ChatSystem.LoggedUserName;
			var users = CSVUsers
				.Split(new char[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(user => user.Trim())
				.Where(user => !String.IsNullOrWhiteSpace(user))
				.ToList();
			if (!users.Contains(currentUser))
			{
				users.Add(currentUser);
			}
			return users;
		}
	}
	/// <summary>
	/// Command for creating new conversation to be bound to a button
	/// </summary>
	public CreateConversationCommand CreateConversation { get; set; }

	public ConversationCreatorViewModel(MainWindow window)
	{
		this.window = window;
		CreateConversation = new CreateConversationCommand(canCreate, Create);
	}

	/// <summary>
	/// Decides, wheter new Conversation can be created
	/// </summary>
	/// <returns></returns>
	bool canCreate()
	{
		return !(String.IsNullOrWhiteSpace(CSVUsers) && String.IsNullOrWhiteSpace(conversationName));
	}

	/// <summary>
	/// Creates new conversation in system, and sends the info to the server
	/// </summary>
	void Create()
	{
		window.OnConversationAdded(
			conversationName, 
			usernames.ToArray());
		CSVUsers = "";
		ConversationName = "";
		window.MainFrame.NavigationService.GoBack();
	}
}