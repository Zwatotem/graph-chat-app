﻿using ChatModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace GraphChatApp.ViewModel;

internal class MessageViewerViewModel : MessageViewModel
{
	private Message message;
	private Conversation conversation;
	MessageViewerViewModel parentMessage;
	MessageViewerViewModel leftBrother;
	MessageViewerViewModel rightBrother;
	private ClientChatSystem chatSystem;

	public bool HasChildren
	{
		get { return DirectChildren.Count > 0; }
	}

	private bool isResponseOpened = false;

	public override bool CanShowMessageEditor
	{
		get { return !isResponseOpened; }
	}

	private ShowMessageEditorCommand showMessageEditorCommand;

	public ICommand AddResponseCommand
	{
		get { return showMessageEditorCommand ?? (showMessageEditorCommand = new ShowMessageEditorCommand(this)); }
	}

	public MessageViewerViewModel(Message message, Conversation conversation)
	{
		ClientChatSystem chatSystem = App.Current.ChatSystem;
		this.message = message;
		if (this.message != null && this.message.Content != null)
		{
			this.message.Content.ContentViewModelProvider = App.Current.ContentViewModelProvider;
		}

		this.conversation = conversation;
		this.chatSystem = chatSystem;
		childrenMessages = new ObservableCollection<MessageViewModel>(
			this.conversation
				.Messages
				.Where(m => m.Parent == this.message)
				.Select(m => new MessageViewerViewModel(m, this.conversation, chatSystem))
				.ToList()
		);
		this.conversation.PropertyChanged += Conversation_PropertyChanged;
	}

	public MessageViewerViewModel(Message message, Conversation conversation, ClientChatSystem chatSystem)
	{
		this.message = message;
		if (this.message != null && this.message.Content != null)
		{
			this.message.Content.ContentViewModelProvider = App.Current.ContentViewModelProvider;
		}
		this.conversation = conversation;
		this.chatSystem = chatSystem;
		childrenMessages = new ObservableCollection<MessageViewModel>(
			this.conversation
				.Messages
				.Where(m => m.Parent == this.message)
				.Select(m => new MessageViewerViewModel(m, this.conversation, chatSystem))
				.ToList()
		);
		this.conversation.PropertyChanged += Conversation_PropertyChanged;
	}

	private void Conversation_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Conversation.ObservableMessages) ||
			e.PropertyName == nameof(Conversation.Messages))
		{
			updateDirectChildren();
		}
	}

	private void updateDirectChildren()
	{
		bool isChanged = false;
		var newRootMessages = this.conversation
			.Messages
			.Where(m => m.Parent == null)
			.Select(m => new MessageViewerViewModel(m, conversation))
			.ToList();

		var oldDirectChildren = this.DirectChildren.ToList();

		foreach (var newDirectChild in newRootMessages)
		{
			if (!oldDirectChildren.Contains(newDirectChild))
			{
				this.DirectChildren.Add(newDirectChild);
				isChanged = true;
			}
		}

		foreach (var oldDirectChild in oldDirectChildren)
		{
			if (!newRootMessages.Contains(oldDirectChild))
			{
				this.DirectChildren.Remove(oldDirectChild);
				isChanged = true;
			}
		}

		if (isChanged)
		{
			OnPropertyChanged(this, new(nameof(DirectChildren)));
		}
	}

	public override string Author
	{
		get { return message.Author.Name; }
	}

	public ChatModel.Util.ViewModel Content
	{
		get { return message.Content.getViewerViewModel(); }
		set { ; }
	}

	public ObservableCollection<MessageViewModel> DirectChildren
	{
		get { return childrenMessages; }
	}

	public void AddResponse()
	{
		var dmvm = new MessageCompositorViewModel(this.message.ID, new TextContent(), this.conversation);
		DirectChildren.Add(dmvm);
		isResponseOpened = true;
	}
}