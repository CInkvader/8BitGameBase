using _8BitGameBase.Backend;
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

        public MainWindow()
        {
            DataContext = this;

            LeaderboardManager.InitializeLeaderboard();
            InitializeComponent();

            _frame = MainFrame;
            _frame.Content = new MainMenu();
        }
        public static void ChangeScreen(Page page)
        {
            _frame.Content = page;
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}