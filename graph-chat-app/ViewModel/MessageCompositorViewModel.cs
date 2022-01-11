using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ChatModel;

namespace GraphChatApp.ViewModel;

public class MessageCompositorViewModel : MessageViewModel
{
	public ClientChatSystem ChatSystem { get; init; }
	public Conversation Conversation { get; init; }
	private Guid parentMessageID;
	public override string Author { get; }
	public override string tempText { get; set; }
	public ICommand SendMessageCommand { get; set; }
	internal MessageCompositorViewModel(Guid parentMessageID, Conversation conversation)
	{
		ClientChatSystem chatSystem = App.Current.ChatSystem;
		Author = chatSystem.LoggedUserName;
		tempText = "";
		this.parentMessageID = parentMessageID;
		ChatSystem = chatSystem;
		Conversation = conversation;
		SendMessageCommand = new SendMessageCommand(this);
	}
	internal MessageCompositorViewModel(Guid parentMessageID, Conversation conversation, ClientChatSystem chatSystem)
	{
		Author = chatSystem.LoggedUserName;
		tempText = "";
		this.parentMessageID = parentMessageID;
		ChatSystem = chatSystem;
		Conversation = conversation;
		SendMessageCommand = new SendMessageCommand(this);
	}

	public void SendMessage()
	{
		var currentUser = ChatSystem.GetUser(ChatSystem.LoggedUserName);
		App.Current.Client
			.requestSendTextMessage(this, new(currentUser, Conversation.ID, parentMessageID, tempText));
	}
}

internal class SendMessageCommand : ICommand
{
	MessageCompositorViewModel viewModel;

	public SendMessageCommand(MessageCompositorViewModel viewModel)
	{
		this.viewModel = viewModel;
	}

	public event EventHandler CanExecuteChanged;

	public bool CanExecute(object parameter)
	{
		return true;
		if (parameter is string s)
		{
			if (String.IsNullOrWhiteSpace(s))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void Execute(object parameter)
	{
		viewModel.SendMessage();
	}
}

