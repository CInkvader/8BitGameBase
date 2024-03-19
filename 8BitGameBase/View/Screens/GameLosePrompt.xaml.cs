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
        private double _difficultyMultiplier = 0;
        private bool _isRecordSaved = false;
        private bool _isRetryOption = false;

        private ScoreRecord _playerRecord = new();

        public int PlayerScore
        {
            get { return _playerScore; }
            set { _playerScore = value; OnPropertyChanged(); }
        }

        public GameLosePrompt(object data, double multiplier, object scoreRecord)
        {
            DataContext = this;
            _previousPage = data ?? new Menu();

            ScoreRecord playerRecord = (ScoreRecord)scoreRecord;
            _playerRecord = playerRecord;

            _difficultyMultiplier = multiplier;
            PlayerScore = playerRecord.Score;

            InitializeComponent();

            MainWindow.MenuBGM.Play();
            InitializeConfirmationPrompt();
            ucSavePrompt.BtnSavePromptBack.Click += BtnSavePromptBack_Click;
            ucSavePrompt.BtnSavePromptSave.Click += BtnSavePromptSave_Click;
        }
        private void InitializeConfirmationPrompt()
        {
            ucConfirmationPrompt.BtnOption1.Width += 20;
            ucConfirmationPrompt.BtnOption2.Width += 20;

            ucConfirmationPrompt.PromptDescription = "SCORE HAS NOT YET BEEN  SAVED";
            ucConfirmationPrompt.FirstOptionContent = "DISCARD SCORE";
            ucConfirmationPrompt.SecondOptionContent = "SAVE SCORE";

            ucConfirmationPrompt.BtnOption1.Click += BtnConfirmOption1_Click;
            ucConfirmationPrompt.BtnOption2.Click += BtnConfirmOption2_Click;
        }

        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            _isRetryOption = true;
            if (!_isRecordSaved)
            {
                ConfirmPrompt();
                return;
            }

            if (_previousPage != null)
            {
                MainWindow.StopMenuSoundMedia();
                Page previousPage = (Page)_previousPage;
                MainWindow.ChangeScreen(new MainGame(previousPage, (DifficultySelection.GameDifficulty)_playerRecord.Difficulty, _difficultyMultiplier));
            }
        }
        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            _isRetryOption = false;
            if (!_isRecordSaved)
            {
                ConfirmPrompt();
                return;
            }
            
            Page previousPage = _previousPage == null ? new MainMenu() : (Page)_previousPage;
            MainWindow.ChangeScreen(previousPage);
        }
        private void BtnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            stpLosePrompt.Visibility = Visibility.Collapsed;
            ucSavePrompt.Visibility = Visibility.Visible;
        }
        private void ConfirmPrompt()
        {
            stpLosePrompt.Visibility = Visibility.Collapsed;

            BtnConfirmPromptBack.Visibility = Visibility.Visible;
            ucConfirmationPrompt.Visibility = Visibility.Visible;
        }

        private void BtnSavePromptBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ucSavePrompt.NameInput = string.Empty;
            ucSavePrompt.Visibility = Visibility.Collapsed;
            stpLosePrompt.Visibility = Visibility.Visible;
        }
        private void BtnSavePromptSave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();

            if (ucSavePrompt.NameInput.Length == 0)
            {
                return;
            }
            _playerRecord.PlayerName = ucSavePrompt.NameInput;
            LeaderboardManager.AddToLeaderboard(_playerRecord);
            _isRecordSaved = true;

            ucSavePrompt.NameInput = string.Empty;
            stpLosePrompt.IsEnabled = true;
            ucSavePrompt.Visibility = Visibility.Collapsed;
            BtnSaveRecord.Visibility = Visibility.Collapsed;
            stpLosePrompt.Visibility = Visibility.Visible;
        }

        private void BtnConfirmOption1_Click(object sender, RoutedEventArgs e)
        {
            _isRecordSaved = true; // To simply override events below and change page
            if (_isRetryOption)
            {
                BtnRetry_Click(BtnRetry, e);
            }
            else
            {
                BtnMenu_Click(BtnMenu, e);
            }
        }
        private void BtnConfirmOption2_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            BtnConfirmPromptBack.Visibility = Visibility.Collapsed;
            ucConfirmationPrompt.Visibility = Visibility.Collapsed;
            ucSavePrompt.Visibility = Visibility.Visible;
        }
        private void BtnConfirmPromptBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            BtnConfirmPromptBack.Visibility = Visibility.Collapsed;
            ucConfirmationPrompt.Visibility = Visibility.Collapsed;
            stpLosePrompt.Visibility = Visibility.Visible;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
