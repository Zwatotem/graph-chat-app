using ChatModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GraphChatApp.ViewModel
{
	class ConversationCollectionViewModel : ViewModel
	{
		private ObservableCollection<ConversationViewModel> conversations;
		private ChatSystem chatSystem;
		private Action<Conversation> enterConversation;

		public ConversationCollectionViewModel(ChatSystem chatSystem, Action<Conversation> enterConversation)
		{
			this.chatSystem = chatSystem;
			chatSystem.PropertyChanged += OnPropertyChanged;
			this.enterConversation = enterConversation;
			this.conversations = new ObservableCollection<ConversationViewModel>(
					chatSystem.Conversations.Select(conv => new ConversationViewModel(conv.Value, enterConversation)).ToList()
				);
		}

		public ObservableCollection<ConversationViewModel> observableConversations
		{
			get
			{
				if (chatSystem.Conversations.Count != conversations.Count)
				{
					this.conversations = new ObservableCollection<ConversationViewModel>(
							chatSystem.ObservableConversations.Select(conv => new ConversationViewModel(conv, enterConversation)).ToList()
						);
				}
				return conversations;
			}
		}
	}
}
