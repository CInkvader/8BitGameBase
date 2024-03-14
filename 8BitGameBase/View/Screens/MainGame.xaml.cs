﻿using _8BitGameBase.Backend;
using _8BitGameBase.View.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _8BitGameBase.View.Screens
{
    public partial class MainGame : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Random _random = new();
        private DispatcherTimer _timer = new();
        private readonly List<BitButton> _buttons = [];

        private int _bitAnswer = 0;
        private int _roundStartTime = 30;
        private int _currentRound = 0;
        private int _playerScore = 0;

        private string _tbDecimalQuestion = string.Empty;
        private string _tbGameTimer = string.Empty;
        private string _tbGameRound = string.Empty;

        public MainGame()
        {
            DataContext = this;

            _timer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 1, 0)
            };
            _timer.Tick += Timer_Tick;

            _buttons = [];
            _tbDecimalQuestion = string.Empty;
            _bitAnswer = 0;

            TbGameRound = "0";
            PlayerScore = 0;

            InitializeComponent();
            InitializeButtons();

            StartGame();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            int time = (int.Parse(TbGameTimer) - 1);
            TbGameTimer = time.ToString();

            if (time == 0)
            {
                StopGame();
            }
        }
        private void InitializeButtons()
        {
            for (int i = 0, j = 128; i < 8; ++i)
            {
                BitButton button = new BitButton();
                _buttons.Add(button);
                _buttons[i].BtnBit.Click += BtnBitClicked;
                _buttons[i].BtnBit.MouseEnter += BtnBitMouseEnter;
                _buttons[i].BtnBit.MouseLeave += BtnBitMouseLeave;
                _buttons[i].BitValue = (j /= i == 0 ? 1 : 2);

                ugButtons.Children.Add(button);
            }

            LosePrompt.BtnRetry.Click += BtnRetry_Click;
            LosePrompt.BtnMenu.Click += BtnMenu_Click;
            LosePrompt.BtnSaveRecord.Click += BtnSaveRecord_Click;
            SavePrompt.BtnSavePromptBack.Click += BtnSavePromptBack_Click;
            SavePrompt.BtnSavePromptSave.Click += BtnSavePromptSave_Click;
        }

        public string TbDecimalQuestion
        {
            get { return _tbDecimalQuestion; }
            set { _tbDecimalQuestion = value; OnPropertyChanged(); }
        }
        public string TbGameTimer
        {
            get { return _tbGameTimer; }
            set { _tbGameTimer = value; OnPropertyChanged(); }
        }
        public string TbGameRound
        {
            get { return _tbGameRound; }
            set
            {
                _currentRound = int.Parse(value);
                _tbGameRound = "Round " + value;
                OnPropertyChanged();
            }
        }
        public int PlayerScore
        {
            get { return _playerScore; }
            set { _playerScore = value; OnPropertyChanged(); }
        }
        
        private void CheckAnswer()
        {
            if (_bitAnswer.ToString() == _tbDecimalQuestion)
            {
                CalculateScore();
                NewRound();
            }
        }
        private void StartGame()
        {
            NewRound();
            _timer.Start();
        }
        private void NewRound()
        {
            ResetBitButtons();
            TbDecimalQuestion = _random.Next(1, 256).ToString();

            TbGameRound = (++_currentRound).ToString();

            if (_currentRound > 1 && _currentRound < 12)
            {
                _roundStartTime -= 2;
            }
            TbGameTimer = _roundStartTime.ToString();
        }
        private void CalculateScore()
        {
            PlayerScore += (int)(50 * (1 + (((double)_currentRound - 1) / 10) + (double.Parse(TbGameTimer) / _roundStartTime)));
        }
        private void ResetBitButtons()
        {
            foreach (BitButton button in _buttons)
            {
                button.BtnContent = "0";
            }
        }
        private void StopGame()
        {
            _timer.Stop();

            GridGame.IsEnabled = false;
            AnimateLosePrompt();
        }
        private void AnimateLosePrompt()
        {
            LosePrompt.Visibility = Visibility.Visible;
        }
        
        private void BtnBitClicked(object? sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            int i = 0; int bitValue = 0;
            foreach (BitButton button in _buttons)
            {
                if (button.BtnBit.Equals((Button)sender))
                {
                    button.SetBit();
                }
                bitValue += button.BtnBitValue;
                ++i;
            }
            _bitAnswer = bitValue;
            CheckAnswer();
        }
        private void BtnBitMouseEnter(object? sender, MouseEventArgs e)
        {
            Button? button = sender as Button;
            if (button == null)
            {
                return;
            }

            int originalDimension = 100;
            int newDimension = 105;

            DoubleAnimation heightAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            DoubleAnimation widthAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0.1)
            };

            Storyboard.SetTarget(heightAnimation, button);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("(FrameworkElement.Height)"));
            Storyboard.SetTarget(widthAnimation, button);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("(FrameworkElement.Width)"));

            Storyboard storyboard = new();
            storyboard.Children.Add(heightAnimation);
            storyboard.Children.Add(widthAnimation);

            storyboard.Begin();
        }
        private void BtnBitMouseLeave(object? sender, MouseEventArgs e)
        {
            Button? button = sender as Button;
            if (button == null)
            {
                return;
            }

            int originalDimension = 105;
            int newDimension = 100;

            DoubleAnimation heightAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            DoubleAnimation widthAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0.1)
            };

            Storyboard.SetTarget(heightAnimation, button);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("(FrameworkElement.Height)"));
            Storyboard.SetTarget(widthAnimation, button);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("(FrameworkElement.Width)"));

            Storyboard storyboard = new();
            storyboard.Children.Add(heightAnimation);
            storyboard.Children.Add(widthAnimation);

            storyboard.Begin();
        }

        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainGame());
        }
        private void BtnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            LosePrompt.Visibility = Visibility.Collapsed;
            SavePrompt.Visibility = Visibility.Visible;
        }
        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainMenu());
        }

        private void BtnSavePromptBack_Click(object sender, RoutedEventArgs e)
        {
            SavePrompt.NameInput = string.Empty;

            SavePrompt.Visibility = Visibility.Collapsed;
            LosePrompt.Visibility = Visibility.Visible;
        }
        private void BtnSavePromptSave_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardManager.AddToLeaderboard(SavePrompt.NameInput, PlayerScore);
            MainWindow.ChangeScreen(new MainMenu());
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
