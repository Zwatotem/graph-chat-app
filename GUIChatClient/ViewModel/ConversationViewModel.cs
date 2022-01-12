using ChatModel;
using System;
using System.ComponentModel;
using System.Linq;
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
class ConversationViewModel : ChatModel.Util.ViewModel
{
	private Conversation conversation;
	private Action<Conversation> enteringMethod;
	public int Count => conversation.Users.Count();

	public ConversationViewModel(Conversation conversation, Action<Conversation> action)
	{
		this.conversation = conversation;
		conversation.PropertyChanged += OnPropertyChanged;
		PropertyChanged += TryUpdateCount;
		this.enteringMethod = action;
		EnterCommand = new EnterConversationCommand(EnterConversation);
	}

	public string Name => conversation.Name;

	private void TryUpdateCount(object sender, PropertyChangedEventArgs e)
	{
		if(e.PropertyName==nameof(conversation))
		{
			if (Count != conversation.Users.Count())
			{
				OnPropertyChanged(this, new(nameof(Count)));
			}
		}
	}
	public EnterConversationCommand EnterCommand { get; set; }
	void EnterConversation()
	{
		enteringMethod(conversation);
	}
}