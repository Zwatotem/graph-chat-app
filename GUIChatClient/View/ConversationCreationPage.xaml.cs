using GraphChatApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphChatApp
{
	/// <summary>
	/// Interaction logic for ConversationCreationPage.xaml
	/// </summary>
	public partial class ConversationCreationPage : Page
	{
		MainWindow window;
		ConversationCreatorViewModel viewModel;
		public ConversationCreationPage(MainWindow window)
		{
			InitializeComponent();
			this.window = window;
			viewModel = new ConversationCreatorViewModel(window);
			DataContext = viewModel;
		}
	}
}
