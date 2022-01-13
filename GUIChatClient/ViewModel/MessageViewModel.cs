using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace GraphChatApp.ViewModel
{
	public abstract class MessageViewModel : ChatModel.Util.ViewModel
	{
		protected ObservableCollection<MessageViewModel> childrenMessages;
		protected const float messageWidth = 100;
		protected const float messageHeight = 80;
		protected const float messageMargin = 10;
		public bool IsWrite { get; } = false;

		public Visibility ChildrenVisibility => !IsWrite ? Visibility.Visible : Visibility.Collapsed;
		public Visibility EditorVisibility => !IsWrite ? Visibility.Collapsed : Visibility.Visible;

		public abstract string Author { get; }

		public virtual bool CanShowMessageEditor
		{
			get { return false; }
		}

		//public static float MessageWidth => messageWidth;
		//public static float MessageHeight => messageHeight;
		//public static float MessageMargin => messageMargin;
	}
}