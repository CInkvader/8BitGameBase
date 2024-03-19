using _8BitGameBase.View.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace _8BitGameBase.View.Screens
{
    public partial class MainMenu : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public MainMenu()
        {
            DataContext = this;
            _btnContent = "Start Game";
            InitializeComponent();
        }

        private string _btnContent = string.Empty;

        public string BtnContent
        {
            get { return _btnContent; }
            set { _btnContent = value; OnPropertyChanged(); }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainGame(this));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
<<<<<<< HEAD

        private void BtnLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new Leaderboard(this));
        }
=======
>>>>>>> parent of 3b05cd5 (Added leaderboard, post-game views and updated menu screen.)
    }
}
