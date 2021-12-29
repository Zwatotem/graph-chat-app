using ChatModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphChatApp.ViewModel
{
	internal class MessageViewModel : ViewModel
	{
		const float messageWidth = 100;
		const float messageHeight = 80;
		const float messageMargin = 10;
		public Message message;
		MessageViewModel parentMessage;
		MessageViewModel leftBrother;
		MessageViewModel rightBrother;
		ObservableCollection<MessageViewModel> childrenMessages;
		public string tempText
		{
			get
			{
				return (message.Content as TextContent).getData() as string;
			}
		}
		int messageLevel
		{
			get
			{
				if (parentMessage == null)
				{
					return 0;
				}
				return parentMessage.messageLevel + 1;
			}
		}
		int messageOrder
		{
			get
			{
				return 0;
			}
		}
		float messageY => messageLevel * (messageHeight + messageMargin) + messageMargin;
		float messageX => messageOrder * (messageWidth + messageMargin) + messageMargin;
		public MessageViewModel(Message message)
		{
			this.message = message;
		}
		public bool Link(List<MessageViewModel> vmMessages)
		{
			var hasParent = vmMessages.Exists(vm => vm.message == message.Parent);
			if (hasParent)
			{
				parentMessage = vmMessages.Find(vm => vm.message == message.Parent);
				parentMessage.childrenMessages.Add(this);
			}
			else
			{
				parentMessage = null;
			}
			return hasParent;
		}

		internal bool IsLinked()
		{
			return (message.TargetId == -1 && parentMessage == null) || parentMessage != null;
		}
	}
}
