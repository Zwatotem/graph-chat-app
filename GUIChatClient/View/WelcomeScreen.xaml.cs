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
	/// Interaction logic for WelcomeScreen.xaml
	/// </summary>
	public partial class WelcomeScreen : Page
	{
		MainWindow window;
		public WelcomeScreen(MainWindow window)
		{
			this.window = window;
			InitializeComponent();
		}

		private void RegisterClick(object sender, RoutedEventArgs e)
		{
			window.OpenRegistrationPage();
		}

		private void LoginClick(object sender, RoutedEventArgs e)
		{
			window.OpenLoginPage();
		}
	}
}
