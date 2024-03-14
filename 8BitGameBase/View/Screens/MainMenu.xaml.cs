using _8BitGameBase.Backend;
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
        private string _btnPlayContent = string.Empty;
        private string _btnLeaderboardContent = string.Empty;


        public MainMenu()
        {
            DataContext = this;
            _btnPlayContent = "Start Game";
            _btnLeaderboardContent = "Leaderboard";
            InitializeComponent();
            LeaderboardManager.InitializeLeaderboard();
        }

        public string BtnPlayContent
        {
            get { return _btnPlayContent; }
            set { _btnPlayContent = value; OnPropertyChanged(); }
        }

        public string BtnLeaderboardContent
        {
            get { return _btnLeaderboardContent; }
            set { _btnLeaderboardContent = value; OnPropertyChanged(); }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainGame());
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new Leaderboard());
        }
    }
}
