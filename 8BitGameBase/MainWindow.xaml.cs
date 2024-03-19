using _8BitGameBase.View.Screens;
using _8BitGameBase.View.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace _8BitGameBase
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static Frame _frame = new();
        private string _BtnMinimizeSymbol = string.Empty;

        public string BtnMinimizeSymbol
        {
            get { return _BtnMinimizeSymbol; }
            set { _BtnMinimizeSymbol = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();

            BtnMinimizeSymbol = "🗖";
            _frame = MainFrame;
            _frame.Content = new MainMenu();
        }

        public static void ChangeScreen(Page page)
        {
            _frame.Content = page;
        }
<<<<<<< HEAD

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                BtnMinimizeSymbol = "🗖";
            }
            else
            {
                WindowState = WindowState.Maximized;
                BtnMinimizeSymbol = "🗗";
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

=======
>>>>>>> parent of 3b05cd5 (Added leaderboard, post-game views and updated menu screen.)
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}