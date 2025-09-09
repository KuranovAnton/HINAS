using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HINAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavigateToPlaner_Click(object sender, RoutedEventArgs e)
        {
            var planerPage = new Planer();

            var navWindow = new NavigationWindow
            {
                Title = "Планировщик путешествий",
                Width = this.Width,
                Height = this.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = planerPage
            };

            this.Close();

            navWindow.Show();
        }
    }
}