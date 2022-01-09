using System.ComponentModel;

namespace GraphChatApp.ViewModel
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
		public void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
		{
			PropertyChanged(this, eventArgs);
		}
	}
}
