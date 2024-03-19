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
    public partial class MainGame : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly object? _previousPage = null;

        private Random _random = new();
        private DispatcherTimer _timer = new();
        private readonly List<BitButton> _buttons = [];

        private int _bitAnswer = 0;
        private int _roundStartTime = 30;

        private string _tbDecimalQuestion = string.Empty;
        private string _tbGameTimer = string.Empty;
        private string _tbGameRound = string.Empty;

        public MainGame(object previousPage)
        {
            _previousPage = previousPage ?? new Menu();
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
            set { _tbGameRound = "Round " + value; OnPropertyChanged(); }
        }

        private void CheckAnswer()
        {
            if (_bitAnswer.ToString() == _tbDecimalQuestion)
            {
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

            // Same as substring
            int round = int.Parse(TbGameRound[(TbGameRound.IndexOf(' ') + 1)..]) + 1;
            TbGameRound = round.ToString();

            if (round > 1 && round < 12)
            {
                _roundStartTime -= 2;
            }
            TbGameTimer = _roundStartTime.ToString();
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
                MainWindow.ChangeScreen(new GameLosePrompt(previousPage, PlayerScore));
            }
        }

<<<<<<< HEAD
=======
            foreach(BitButton button in _buttons)
            {
                button.BtnBit.IsEnabled = false;
            }
            LosePrompt.Visibility = Visibility.Visible;
        }

>>>>>>> parent of 3b05cd5 (Added leaderboard, post-game views and updated menu screen.)
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
<<<<<<< HEAD
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

=======
>>>>>>> parent of 3b05cd5 (Added leaderboard, post-game views and updated menu screen.)
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
