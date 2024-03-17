using _8BitGameBase.Backend;
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

namespace _8BitGameBase.View.Screens
{
    public partial class GameLosePrompt : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly object? _previousPage = null;
        private int _playerScore = 0;
        private int _selectedDifficulty = 0;
        private double _difficultyMultiplier = 0;

        public GameLosePrompt(object data, DifficultySelection.GameDifficulty gameDifficulty, double multiplier, int playerScore = 0)
        {
            DataContext = this;
            _previousPage = data ?? new Menu();
            _selectedDifficulty = (int)gameDifficulty;
            _difficultyMultiplier = multiplier;

            InitializeComponent();
            PlayerScore = playerScore;

            SavePrompt.BtnSavePromptBack.Click += BtnSavePromptBack_Click;
            SavePrompt.BtnSavePromptSave.Click += BtnSavePromptSave_Click;
        }
        public int PlayerScore
        {
            get { return _playerScore; }
            set { _playerScore = value; OnPropertyChanged(); }
        }

        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            if (_previousPage != null)
            {
                Page previousPage = (Page)_previousPage;
                MainWindow.ChangeScreen(new MainGame(previousPage, (DifficultySelection.GameDifficulty)_selectedDifficulty, _difficultyMultiplier));
            }
        }
        private void BtnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            stpLosePrompt.IsEnabled = false;
            stpLosePrompt.Visibility = Visibility.Collapsed;
            SavePrompt.Visibility = Visibility.Visible;
        }
        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainMenu());
        }

        private void BtnSavePromptBack_Click(object sender, RoutedEventArgs e)
        {
            SavePrompt.NameInput = string.Empty;
            stpLosePrompt.IsEnabled = true;
            SavePrompt.Visibility = Visibility.Collapsed;
            stpLosePrompt.Visibility = Visibility.Visible;
        }
        private void BtnSavePromptSave_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardManager.AddToLeaderboard(SavePrompt.NameInput, PlayerScore);
            if (_previousPage != null)
            {
                Page previousPage = (Page)_previousPage;
                MainWindow.ChangeScreen(previousPage);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
