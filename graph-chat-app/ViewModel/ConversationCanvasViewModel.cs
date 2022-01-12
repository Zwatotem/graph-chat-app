﻿using ChatModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace GraphChatApp.ViewModel;

internal class ConversationCanvasViewModel : ChatModel.Util.ViewModel
{
	public readonly Conversation conversation;
	private string conversationName;
	ObservableCollection<MessageViewModel> rootMessages;
	private ClientChatSystem chatSystem;
	public ClientChatSystem ChatSystem
	{
		get { return chatSystem; }
	}
	public string ConversationName => conversationName;
	private MessageCompositorViewModel globalEditor;
	public MessageCompositorViewModel GlobalEditor
	{
		get
		{
			return globalEditor;
		}
		set
		{
			globalEditor = value;
			if(globalEditor != null)
			{
				rootMessages.Add(GlobalEditor);
			}
			else
			{
				rootMessages.Remove(GlobalEditor);
			}
		}
	}

	public ICommand ShowGlobalEditorCommand { get; set; }

	public ObservableCollection<MessageViewModel> RootMessages
	{
		get { return rootMessages; }
	}

	public ConversationCanvasViewModel(Conversation conversation)
	{
		ClientChatSystem chatSystem = App.Current.ChatSystem;
		this.conversation = conversation;
		rootMessages = new ObservableCollection<MessageViewModel>(
			this.conversation
				.Messages
				.Where(m => m.Parent == null)
				.Select(m => new MessageViewerViewModel(m, this.conversation, this.chatSystem))
				.ToList()
		);
		this.conversationName = conversation.Name;
		this.chatSystem = chatSystem;
		this.conversation.PropertyChanged += Conversation_PropertyChanged;
		this.globalEditor = null;
		this.ShowGlobalEditorCommand = new ShowGlobalEditorCommand(this);
	}
	public ConversationCanvasViewModel(Conversation conversation, ClientChatSystem chatSystem)
	{
		this.conversation = conversation;
		rootMessages = new ObservableCollection<MessageViewModel>(
			this.conversation
				.Messages
				.Where(m => m.Parent == null)
				.Select(m => new MessageViewerViewModel(m, this.conversation, this.chatSystem))
				.ToList()
		);
		this.conversationName = conversation.Name;
		this.chatSystem = chatSystem;
		this.conversation.PropertyChanged += Conversation_PropertyChanged;
		this.globalEditor = null;
		this.ShowGlobalEditorCommand = new ShowGlobalEditorCommand(this);
	}

	private void Conversation_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Conversation.Name))
		{
			this.conversationName = this.conversation.Name;
			OnPropertyChanged(this, new(nameof(ConversationName)));
		}

		if (e.PropertyName == nameof(Conversation.ObservableMessages) ||
			e.PropertyName == nameof(Conversation.Messages))
		{
			updateRootMessages();
		}
	}

	private void updateRootMessages()
	{
		bool isChanged = false;
		var newRootMessages = this.conversation
			.Messages
			.Where(m => m.Parent == null)
			.Select(m => new MessageViewerViewModel(m, conversation, this.chatSystem))
			.ToList();

		var oldRootMessages = this.rootMessages.ToList();

		foreach (var newRootMessage in newRootMessages)
		{
			if (!oldRootMessages.Contains(newRootMessage))
			{
				this.rootMessages.Add(newRootMessage);
				isChanged = true;
			}
		}

		foreach (var oldRootMessage in oldRootMessages)
		{
			if (!newRootMessages.Contains(oldRootMessage))
			{
				this.rootMessages.Remove(oldRootMessage);
				isChanged = true;
			}
		}

		if (globalEditor is not null)
		{
			this.rootMessages.Add(globalEditor);
		}

		if (isChanged)
		{
			OnPropertyChanged(this, new(nameof(RootMessages)));
		}
	}
}

internal class ShowGlobalEditorCommand : ICommand
{
	private ConversationCanvasViewModel viewModel;

	public ShowGlobalEditorCommand(ConversationCanvasViewModel viewModel)
	{
		this.viewModel = viewModel;
	}
	public event EventHandler CanExecuteChanged;

	public bool CanExecute(object parameter)
	{
		return viewModel.GlobalEditor is null;
	}

	public void Execute(object parameter)
	{
		viewModel.GlobalEditor = new MessageCompositorViewModel(Guid.Empty, new TextContent() ,viewModel.conversation, viewModel.ChatSystem);
	}
}