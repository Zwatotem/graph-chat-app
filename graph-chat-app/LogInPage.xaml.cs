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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			window.OnUserLogged(Namebox.Text);
		}

		private void Username_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
