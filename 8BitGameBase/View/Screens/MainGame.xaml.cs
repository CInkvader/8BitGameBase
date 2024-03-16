using _8BitGameBase.Backend;
using _8BitGameBase.View.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Reflection;
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
        private readonly object? _previousPage = null;

        private Random _random = new();
        private DispatcherTimer _timer = new();
        private readonly List<BitButton> _buttons = [];

        private int _selectedDifficulty = 0;
        private int _roundStartTime = 0;
        private int _roundTimeDeduction = 0;
        private int _baseScore = 25;

        private int _bitAnswer = 0;
        private int _currentRound = 0;
        private int _playerScore = 0;

        private string _tbDecimalQuestion = string.Empty;
        private string _tbGameTimer = string.Empty;
        private string _tbGameRound = string.Empty;
        private string _tbDifficulty = string.Empty;

        public MainGame(object previousPage, DifficultySelection.GameDifficulty gameDifficulty)
        {
            _previousPage = previousPage ?? new Menu();
            DataContext = this;
            _selectedDifficulty = (int)gameDifficulty;

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
                GameFinish();
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
        public string TbDifficulty
        {
            get { return _tbDifficulty; }
            set { _tbDifficulty = value; OnPropertyChanged(); }
        }

        public int PlayerScore
        {
            get { return _playerScore; }
            set { _playerScore = value; OnPropertyChanged(); }
        }
        
        private void SetDifficulty()
        {
            switch (_selectedDifficulty)
            {
                case 1:
                    TbDifficulty = "I";
                    _roundStartTime = 90;
                    _roundTimeDeduction = 6;
                    break;
                case 2:
                    TbDifficulty = "II";
                    _roundStartTime = 60;
                    _roundTimeDeduction = 4;
                    break;
                case 3:
                    TbDifficulty = "III";
                    _roundStartTime = 30;
                    _roundTimeDeduction = 2;
                    break;
                case 4:
                    TbDifficulty = "IV";
                    _roundStartTime = 15;
                    _roundTimeDeduction = 1;
                    break;
            }
        }

        private void StartGame()
        {
            SetDifficulty();
            NewRound();
        }
        private void CheckAnswer()
        {
            if (_bitAnswer.ToString() == _tbDecimalQuestion)
            {
                CalculateScore();
                PlayBlinkAnimation();
            }
        }

        private void NewRound()
        {
            ResetBitButtons();
            TbDecimalQuestion = _random.Next(1, 256).ToString();

            TbGameRound = (++_currentRound).ToString();

            if (_currentRound > 1 && _currentRound < 12)
            {
                _roundStartTime -= _roundTimeDeduction;
            }
            TbGameTimer = _roundStartTime.ToString();

            _timer.Start();
        }
        private void CalculateScore()
        {
            double performanceMultiplier = 1 + (((double)_currentRound - 1) / 10) + (double.Parse(TbGameTimer) / _roundStartTime);
            PlayerScore += (int)Math.Ceiling(_selectedDifficulty * _baseScore * performanceMultiplier);
        }
        private void ResetBitButtons()
        {
            foreach (BitButton button in _buttons)
            {
                button.BtnContent = "0";
            }
        }
        private void GameFinish()
        {
            _timer.Stop();
            if (_previousPage != null)
            {
                Page previousPage = (Page)_previousPage;
                MainWindow.ChangeScreen(new GameLosePrompt(previousPage, (DifficultySelection.GameDifficulty)_selectedDifficulty, PlayerScore));
            }
        }

        private void BtnBitClicked(object? sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            int bitValue = 0;
            foreach (BitButton button in _buttons)
            {
                if (button.BtnBit.Equals((Button)sender))
                {
                    button.SetBit();
                }
                bitValue += button.BtnBitValue;
            }
            _bitAnswer = bitValue;
            CheckAnswer();
        }
        private void BtnBitMouseEnter(object? sender, MouseEventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }
            BtnBitResizeAnimation(button, 70, 75);
        }
        private void BtnBitMouseLeave(object? sender, MouseEventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }
            BtnBitResizeAnimation(button, 75, 70);
        }
        private static void BtnBitResizeAnimation(Button button, int from, int to)
        {
            int originalDimension = from;
            int newDimension = to;

            DoubleAnimation heightAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0)
            };
            DoubleAnimation widthAnimation = new()
            {
                From = originalDimension,
                To = newDimension,
                Duration = TimeSpan.FromSeconds(0)
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

        private void BtnMainGameBack_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            MainWindow.ChangeScreen((Page)(_previousPage ?? new MainMenu()));
        }

        private static Storyboard InitializeAnimation()
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = Color.FromArgb(0xFF, 0x5C, 0x2E, 0x78),
                To = Color.FromArgb(0xFF, 0xFF, 0xA8, 0x00),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(0.2),
            };
            Storyboard blinkAnimation = new Storyboard();
            blinkAnimation.Children.Add(colorAnimation);
            blinkAnimation.RepeatBehavior = new RepeatBehavior(3);

            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Background.Color"));
            return blinkAnimation;
        }
        private void PlayBlinkAnimation()
        {
            GridGame.IsEnabled = false;
            int i = 0;
            int limit = _buttons.Count - 1;
            _timer.Stop();

            foreach (BitButton button in _buttons)
            {
                button.BtnBit.IsDefault = true;
                Storyboard blinkingAnimation = InitializeAnimation();

                if (blinkingAnimation == null)
                    continue;

                if (i++ == limit)
                {
                    blinkingAnimation.Completed += BlinkAnimation_Completed;
                }

                Storyboard.SetTarget(blinkingAnimation, button.BtnBit);
                blinkingAnimation.Begin();
            }
        }
        private void BlinkAnimation_Completed(object? sender, EventArgs e)
        {
            NewRound();
            GridGame.IsEnabled = true;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}