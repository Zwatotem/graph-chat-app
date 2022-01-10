using ChatModel;
using System;
using System.Windows.Input;

namespace GraphChatApp.ViewModel;

class EnterConversationCommand : ICommand
{
	public event EventHandler CanExecuteChanged = (sender, e) => { };
	private Action execute;
	public EnterConversationCommand(Action execute)
	{
		this.execute = execute;
	}
	public bool CanExecute(object parameter) => true;
	public void Execute(object parameter) => execute();
}
class ConversationViewModel : ViewModel
{
	private Conversation conversation;
	private Action<Conversation> enteringMethod;

	public ConversationViewModel(Conversation conversation, Action<Conversation> action)
	{
		this.conversation = conversation;
		conversation.PropertyChanged += OnPropertyChanged;
		this.enteringMethod = action;
		EnterCommand = new EnterConversationCommand(EnterConversation);
	}

	public string Name
	{
		get
		{
			return conversation.Name;
		}
	}

	public EnterConversationCommand EnterCommand { get; set; }
	void EnterConversation()
	{
		enteringMethod(conversation);
	}
}