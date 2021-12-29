using ChatModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GraphChatApp.ViewModel
{
	internal class ConversationCanvasViewModel : ViewModel
	{
		Conversation conversation;
		private string conversationName;
		List<MessageViewModel> allMessages;
		ObservableCollection<MessageViewModel> rootMessages;

		public ObservableCollection<Message> observableMessages
		{
			get
			{
				return conversation.observableMessages;
			}
		}

		public ObservableCollection<MessageViewModel> RootMessages
		{
			get
			{
				return rootMessages;
			}
		}

		public ConversationCanvasViewModel(Conversation conversation)
		{
			this.conversation = conversation;
			allMessages = new List<MessageViewModel>();
			rootMessages = new ObservableCollection<MessageViewModel>();
			PropertyChanged += CheckAdditionalPropertyChanged;
			conversation.PropertyChanged += InvokePropertyChanged;
			BuildMessages();
		}

		private void BuildMessages()
		{
			foreach (var m in conversation.Messages)
			{
				allMessages.Add(new MessageViewModel(m));
			}
			foreach (var m in allMessages)
			{
				if (!m.Link(allMessages))
				{
					rootMessages.Add(m);
					InvokePropertyChanged(this, new(nameof(RootMessages)));
				};
			}
		}

		private void RebuildMessages()
		{
			foreach (var m in conversation.Messages)
			{
				if (!allMessages.Exists(vm => vm.message == m))
				{
					allMessages.Add(new MessageViewModel(m));
					InvokePropertyChanged(this, new(nameof(allMessages)));
					InvokePropertyChanged(this, new(nameof(RootMessages)));
				}
			}
			foreach (var m in allMessages)
			{
				if (!m.IsLinked() && !m.Link(allMessages))
				{
					rootMessages.Add(m);
					InvokePropertyChanged(this, new(nameof(RootMessages)));
				};
			}
		}

		public string ConversationName { get => conversation.Name; }

		void CheckAdditionalPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
		{
			var propertyName = eventArgs.PropertyName;
			switch (propertyName)
			{
				case "observableMessages":
					RebuildMessages();
					break;
				default:
					break;
			}
		}
	}
}