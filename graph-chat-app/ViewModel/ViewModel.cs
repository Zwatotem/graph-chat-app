﻿using System.ComponentModel;

namespace GraphChatApp.ViewModel
{
	class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
	}
}