using ChatModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
		get
		{
			return DirectChildren.Count > 0;
		}
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
		if (e.PropertyName == nameof(Conversation.observableMessages) ||
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

	public string Content
	{
		get { return (message.Content as TextContent).getData() as string; }
		set {; }
	}

	public override string tempText { get; set; }

	public ObservableCollection<MessageViewModel> DirectChildren
	{
		get { return childrenMessages; }
	}

	public void AddResponse()
	{
		var dmvm = new MessageCompositorViewModel(this.message.ID, this.conversation);
		DirectChildren.Add(dmvm);
		isResponseOpened = true;
	}
}