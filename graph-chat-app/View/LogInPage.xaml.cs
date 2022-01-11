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
	/// Interaction logic for LogInPage.xaml
	/// </summary>
	public partial class LogInPage : Page
	{
		MainWindow window;
		public LogInPage(MainWindow window)
		{
			this.window = window;
			InitializeComponent();
		}

		private void LoginClick(object sender, RoutedEventArgs e)
		{
			var username = Namebox.Text;
			if (String.IsNullOrWhiteSpace(username))
			{
				ErrorDisplay.Text = "Please enter the username";
			}
			else if (username.Contains(','))
			{
				ErrorDisplay.Text = "Username cannot contain commas (,)";
			}
			else
			{
				window.OnUserLogged(Namebox.Text.Trim());
			}
		}
		
		public void ClearError()
		{
			ErrorDisplay.Text = "";
		}
		
		public void ShowBadUserError()
		{
			ErrorDisplay.Text = $"User name {Namebox.Text} does not exist";
		}
	}
}
