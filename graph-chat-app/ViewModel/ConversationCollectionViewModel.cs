using ChatModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphChatApp.ViewModel
{
	class ConversationCollectionViewModel : ViewModel
	{
		private ObservableCollection<ConversationViewModel> conversations;
		private ChatSystem chatSystem;

		public ConversationCollectionViewModel(ChatSystem chatSystem)
		{
			this.chatSystem = chatSystem;
			this.conversations = new ObservableCollection<ConversationViewModel>(
					chatSystem.Conversations.Select(conv => new ConversationViewModel(conv.Value))
				);
		}


		public ObservableCollection<ConversationViewModel> Conversations
		{
			get
			{
				if (chatSystem.Conversations.Count != conversations.Count)
				{
					this.conversations = new ObservableCollection<ConversationViewModel>(
							chatSystem.Conversations.Select(conv => new ConversationViewModel(conv.Value))
						);
				}
				return conversations;
			}
		}
	}
}
