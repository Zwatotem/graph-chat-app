using ChatModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphChatApp.ViewModel
{
	class ConversationViewModel : ViewModel
	{
		private Conversation conversation;

		public ConversationViewModel(Conversation conversation)
		{
			this.conversation = conversation;
		}

		public string Name
		{
			get
			{
				return conversation.Name;
			}
		}
	}
}
