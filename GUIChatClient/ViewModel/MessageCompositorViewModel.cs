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
	private readonly IMessageContent content;
	public IMessageContent Content
	{
		get => content;
		init => content = value;
	}

	public ChatModel.Util.ViewModel ContentCompositor
	{
		get => content.getCompositorViewModel();
		set => OnPropertyChanged(this, new(nameof(ContentCompositor)));
	}

	public ICommand SendMessageCommand { get; set; }
	internal MessageCompositorViewModel(Guid parentMessageID, IMessageContent contentToBuild , Conversation conversation)
	{
		this.Content = contentToBuild;
		this.Content.ContentViewModelProvider = App.Current.ContentViewModelProvider;
		this.ContentCompositor = content.getCompositorViewModel();
		ClientChatSystem chatSystem = App.Current.ChatSystem;
		Author = chatSystem.LoggedUserName;
		this.parentMessageID = parentMessageID;
		ChatSystem = chatSystem;
		Conversation = conversation;
		SendMessageCommand = new SendMessageCommand(this);
	}
	internal MessageCompositorViewModel(Guid parentMessageID, IMessageContent contentToBuild , Conversation conversation, ClientChatSystem chatSystem)
	{
		this.Content = contentToBuild;
		this.Content.ContentViewModelProvider = App.Current.ContentViewModelProvider;
		this.ContentCompositor = content.getCompositorViewModel();
		Author = chatSystem.LoggedUserName;
		this.parentMessageID = parentMessageID;
		ChatSystem = chatSystem;
		Conversation = conversation;
		SendMessageCommand = new SendMessageCommand(this);
	}

	public void SendMessage()
	{
		var currentUser = ChatSystem.GetUser(ChatSystem.LoggedUserName);
		App.Current.Client
			.requestSendMessage(this, new(currentUser.ID, Conversation.ID, parentMessageID, this.content));
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

