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
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            DataContext = this;

            InitializeComponent();
            LeaderboardManager.InitializeLeaderboard();
            ucGameSettings.BtnSettingsBack.Click += BtnSettingsBack_Click;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            MainWindow.ChangeScreen(new DifficultySelection(this));
        }
        private void BtnLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            MainWindow.ChangeScreen(new Leaderboard(this));
        }
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            StpMenu.Visibility = Visibility.Collapsed;
            ucGameSettings.Visibility = Visibility.Visible;
        }
        private void BtnSettingsBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ucGameSettings.Visibility = Visibility.Collapsed;
            StpMenu.Visibility = Visibility.Visible;
        }
    }
}
