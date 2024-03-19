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
        private readonly MainWindow? _mainWIndow;

        private Random _random = new();
        private DispatcherTimer _timer = new();
        private readonly List<BitButton> _buttons = [];

        private int _selectedDifficulty = 0;
        private double _difficultyMultiplier = 0;

        private int _roundStartTime = 0;
        private int _roundTimeDeduction = 0;
        private int _baseScore = 25;
        private int _minimumDecimal = 0;

        private int _bitAnswer = 0;
        private int _currentRound = 0;
        private int _playerScore = 0;
        private int _totalPlayTime = 0;

        private Storyboard? _barAnimation = null;
        private Storyboard[]? _warningAnimations = [];

        private string _tbDecimalQuestion = string.Empty;
        private string _tbGameTimer = string.Empty;
        private string _tbGameRound = string.Empty;
        private string _tbDifficulty = string.Empty;

        public MainGame(object previousPage, DifficultySelection.GameDifficulty gameDifficulty, double difficultyMultiplier)
        {
            _previousPage = previousPage ?? new Menu();
            DataContext = this;
            _mainWIndow = MainWindow.Instance;

            _selectedDifficulty = (int)gameDifficulty;
            _difficultyMultiplier = difficultyMultiplier;

            _timer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 1, 0)
            };
            _timer.Tick += Timer_Tick;
            _warningAnimations = null;

            _buttons = [];
            _tbDecimalQuestion = string.Empty;
            _bitAnswer = 0;

            TbGameRound = "0";
            PlayerScore = 0;

            InitializeComponent();
            InitializePrompts();
            LoadGame();
        }
        private async void LoadGame()
        {
            ucTutorialScreen.Visibility = Visibility.Visible;

            SetDifficulty();
            SetBGM();
            InitializeBitButtons();

            if (_mainWIndow != null)
                _mainWIndow.BdTimerBar.Background = (SolidColorBrush)FindResource("DefaultColorMedium_3");
            TimerBarPlayAnimation(false, 5);

            await Task.Delay(TimeSpan.FromSeconds(5));
            ucTutorialScreen.Visibility = Visibility.Collapsed;

            GridGame.Visibility = Visibility.Visible;
            GridGame.IsEnabled = true;
            StartGame();
        }
        private void InitializeBitButtons()
        {
            for (int i = 0, j = 128; i < 8; ++i)
            {
                BitButton button = new BitButton();
                _buttons.Add(button);
                _buttons[i].VerticalAlignment = VerticalAlignment.Bottom;
                _buttons[i].Margin = new Thickness(0,0,0,400);
                _buttons[i].BtnBit.Click += BtnBitClicked;
                _buttons[i].BtnBit.MouseEnter += BtnBitMouseEnter;
                _buttons[i].BtnBit.MouseLeave += BtnBitMouseLeave;
                _buttons[i].BitValue = (j /= i == 0 ? 1 : 2);

                ugButtons.Children.Add(button);
            }
        }
        private void InitializePrompts()
        {
            ucConfirmBackPrompt.BtnOption3.Visibility = Visibility.Visible;
            ucConfirmBackPrompt.PromptDescription = "GAME MENU OPTIONS";
            ucConfirmBackPrompt.FirstOptionContent = "BACK  TO GAME";
            ucConfirmBackPrompt.SecondOptionContent = "BACK  TO MENU";
            ucConfirmBackPrompt.ThirdOptionContent = "SETTINGS";
            ucConfirmBackPrompt.BtnOption1.Click += BtnPromptBackToGame_Click;
            ucConfirmBackPrompt.BtnOption2.Click += BtnPromptBackToMenu_Click;
            ucConfirmBackPrompt.BtnOption3.Click += BtnPromptSettings_Click;
            ucGameSettings.BtnSettingsBack.Click += BtnSettingsBack_Click;
        }

        private void SetBGM()
        {
            Uri? gameBGMPath = null;
            switch (_selectedDifficulty)
            {
                case 1:
                    gameBGMPath = new Uri("Media/Sounds/BGM/Music_1.mp3", UriKind.RelativeOrAbsolute); break;
                case 2:
                    gameBGMPath = new Uri("Media/Sounds/BGM/Music_2.mp3", UriKind.RelativeOrAbsolute); break;
                case 3:
                    gameBGMPath = new Uri("Media/Sounds/BGM/Music_3.mp3", UriKind.RelativeOrAbsolute); break;
                default:
                    gameBGMPath = new Uri("Media/Sounds/BGM/Music_4.mp3", UriKind.RelativeOrAbsolute); break;
            }
            MainWindow.GameBGM.Open(gameBGMPath);
        }
        private void SetDifficulty()
        {
            switch (_selectedDifficulty)
            {
                case 1:
                    TbDifficulty = "I";
                    _roundStartTime = 90;
                    _roundTimeDeduction = 6;
                    _minimumDecimal = 1;
                    break;
                case 2:
                    TbDifficulty = "II";
                    _roundStartTime = 60;
                    _roundTimeDeduction = 4;
                    _minimumDecimal = 1;
                    break;
                case 3:
                    TbDifficulty = "III";
                    _roundStartTime = 30;
                    _roundTimeDeduction = 2;
                    _minimumDecimal = 30;
                    break;
                case 4:
                    TbDifficulty = "IV";
                    _roundStartTime = 15;
                    _roundTimeDeduction = 1;
                    _minimumDecimal = 70;
                    break;
            }
        }
        private void StartGame()
        {
            if (_mainWIndow != null)
                _mainWIndow.BdTimerBar.Background = (SolidColorBrush)FindResource("DefaultColorMedium_2");

            MainWindow.GameBGM.Play();
            NewRound();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            int time = (int.Parse(TbGameTimer) - 1);
            TbGameTimer = time.ToString();

            if (time == 0)
            {
                GameFinish();
                return;
            }
            ++_totalPlayTime;
            if (time <= 10 && _warningAnimations == null)
            {
                PlayWarningAnimation();
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
        
        private async void CheckAnswer()
        {
            if (_bitAnswer.ToString() == _tbDecimalQuestion)
            {
                MainWindow.GameCorrectAnswer.Play();

                _timer.Stop();
                StopAnimations();
                PlayBlinkAnimation();

                await Task.Delay(TimeSpan.FromSeconds(1));
                CalculateScore();
                NewRound();
                GridGame.IsEnabled = true;
            }
        }
        private void NewRound()
        {
            ResetBitButtons();
            TbDecimalQuestion = _random.Next(_minimumDecimal, 256).ToString();

            TbGameRound = (++_currentRound).ToString();

            if (_currentRound > 1 && _currentRound < 12)
            {
                _roundStartTime -= _roundTimeDeduction;
            }
            TbGameTimer = _roundStartTime.ToString();

            _timer.Start();
            TimerBarPlayAnimation();
            if (_selectedDifficulty >= 3)
            {
                foreach (BitButton button in _buttons)
                {
                    button.PlayValueFadeAnimation();
                }
            }
        }
        private void CalculateScore()
        {
            double performanceMultiplier = 1 + (((double)_currentRound - 1) / 10) + (double.Parse(TbGameTimer) / _roundStartTime);
            PlayerScore += (int)Math.Ceiling(_difficultyMultiplier * _baseScore * performanceMultiplier);
        }
        private void ResetBitButtons()
        {
            foreach (BitButton button in _buttons)
            {
                button.BtnContent = "0";
            }
        }
        private async void GameFinish()
        {
            _timer.Stop();
            SetGameLoseMedia();

            await Task.Delay(TimeSpan.FromSeconds(2));
            PlayGameFinishAnimation();

            await Task.Delay(TimeSpan.FromSeconds(2.5));
            if (_previousPage != null)
            {
                Page previousPage = (Page)_previousPage;
                DifficultySelection.GameDifficulty difficulty = (DifficultySelection.GameDifficulty)_selectedDifficulty;

                ScoreRecord playerRecord = new ScoreRecord()
                {
                    Score = PlayerScore,
                    Difficulty = (_selectedDifficulty),
                    Playtime = _totalPlayTime,
                    HighestRound = _currentRound
                };

                MainWindow.ChangeScreen(new GameLosePrompt(previousPage, _difficultyMultiplier, playerRecord));
            }
        }
        private async void PlayGameFinishAnimation()
        {
            MainWindow.GameLoseSound.Play();
            TbRound.Visibility = Visibility.Collapsed;

            foreach (BitButton button in _buttons) {
                Thickness bounceMargin = button.Margin;
                bounceMargin.Bottom += 40;

                Thickness dropMargin = bounceMargin;
                bounceMargin.Bottom = 0;

                ThicknessAnimationUsingKeyFrames bounceAnimation = new()
                {
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                bounceAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(dropMargin));
                bounceAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(bounceMargin));

                Storyboard storyboard = new();
                Storyboard.SetTarget(bounceAnimation, button);
                Storyboard.SetTargetProperty(bounceAnimation, new PropertyPath(MarginProperty));

                storyboard.Children.Add(bounceAnimation);
                storyboard.Begin();
                await Task.Delay(TimeSpan.FromSeconds(0.12));
            }
        }
        private void SetGameLoseMedia()
        {
            if (_selectedDifficulty >= 3)
            {
                foreach (BitButton button in _buttons)
                {
                    button.StopAnimation();
                }
            }
            ugButtons.IsEnabled = false;
            MainWindow.GameLoseFallSound.Play();

            ToggleExitPrompt(false);
            ucGameSettings.Visibility = Visibility.Collapsed;
            StopAnimations();
            StopGameMedia();

            BtnMainGameBack.Visibility = Visibility.Collapsed;
            TbDecimalQuestion = "TIME'S UP!";
        }

        private void BtnMainGameBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ToggleExitPrompt(true);
        }
        private void BtnPromptBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            _timer.Stop();
            StopAnimations();
            StopGameMedia();

            MainWindow.ChangeScreen((Page)(_previousPage ?? new MainMenu()));
        }
        private void BtnPromptBackToGame_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ToggleExitPrompt(false);
        }
        private void ToggleExitPrompt(bool showPrompt = true)
        {
            if (showPrompt)
            {
                TbRound.Visibility = Visibility.Collapsed;
                stpGameContents.Visibility = Visibility.Collapsed;
                BtnMainGameBack.Visibility = Visibility.Collapsed;

                ucConfirmBackPrompt.Visibility = Visibility.Visible;
            }
            else
            {
                ucConfirmBackPrompt.Visibility = Visibility.Collapsed;

                TbRound.Visibility = Visibility.Visible;
                stpGameContents.Visibility = Visibility.Visible;
                BtnMainGameBack.Visibility = Visibility.Visible;
            }
        }

        private void BtnSettingsBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ucGameSettings.Visibility = Visibility.Collapsed;
            ucConfirmBackPrompt.Visibility = Visibility.Visible;
        }
        private void BtnPromptSettings_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            ucConfirmBackPrompt.Visibility = Visibility.Collapsed;
            ucGameSettings.Visibility = Visibility.Visible;
        }

        private void BtnBitClicked(object? sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            MainWindow.PlayButtonSound();
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

        private static Storyboard InitializeColorAnimation(Color fromColor, Color toColor, double duration = 0.2, bool reverse = true)
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = fromColor,
                To = toColor,
                AutoReverse = reverse,
                Duration = TimeSpan.FromSeconds(duration),
            };
            Storyboard blinkAnimation = new();
            blinkAnimation.Children.Add(colorAnimation);

            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Background.Color"));
            return blinkAnimation;
        }
        private void PlayBlinkAnimation()
        {
            GridGame.IsEnabled = false;

            foreach (BitButton button in _buttons)
            {
                button.BtnBit.IsDefault = true;
                Storyboard blinkingAnimation = InitializeColorAnimation(Color.FromArgb(0xFF, 0x5C, 0x2E, 0x78), Color.FromArgb(0xFF, 0xFF, 0xA8, 0x00));
                blinkingAnimation.RepeatBehavior = new RepeatBehavior(2);

                if (blinkingAnimation == null)
                    continue;

                Storyboard.SetTarget(blinkingAnimation, button.BtnBit);
                blinkingAnimation.Begin();
            }

            if (_mainWIndow != null)
            {
                Storyboard cornerBlink = InitializeColorAnimation(Color.FromArgb(0xFF, 0x12, 0x12, 0x12), Color.FromArgb(0xFF, 0xFF, 0xA8, 0x00));
                cornerBlink.RepeatBehavior = new RepeatBehavior(2);

                Storyboard.SetTarget(cornerBlink, _mainWIndow.BdCornerLight);
                cornerBlink.Begin();
            }
        }
        private void PlayWarningAnimation()
        {
            if (_mainWIndow == null)
                return;

            _warningAnimations = new Storyboard[3];

            for (int i = 0; i < 3; ++i)
            {
                _warningAnimations[i] = InitializeColorAnimation(Color.FromArgb(0xFF, 0x12, 0x12, 0x12), Color.FromArgb(0xFF, 0x9C, 0x1D, 0x1D), 0.25);
                _warningAnimations[i].RepeatBehavior = new RepeatBehavior(20);
            }

            Storyboard.SetTarget(_warningAnimations[0], _mainWIndow.BdWarning);
            Storyboard.SetTarget(_warningAnimations[1], _mainWIndow.BdCornerLight);
            Storyboard.SetTarget(_warningAnimations[2], _mainWIndow.BdTimer);

            for (int i = 0; i < 3; ++i)
            {
                _warningAnimations[i].Begin();
            }
        }

        private void TimerBarPlayAnimation(bool enableColorAnimation = true, double? duration = null)
        {
            if (_mainWIndow == null)
                return;

            double barDuration = 0;
            if (duration != null)
                barDuration = duration.Value;
            else
                barDuration = _roundStartTime;

            double startLength = (double)this.FindResource("timerBarLength");

            DoubleAnimation widthAnimation = new()
            {
                From = startLength,
                To = 0,
                Duration = TimeSpan.FromSeconds(barDuration)
            };
            Storyboard barAnimation = new Storyboard();
            if (enableColorAnimation)
                barAnimation = InitializeColorAnimation(Color.FromArgb(0xFF, 0x21, 0x71, 0x43), Color.FromArgb(0xFF, 0x9C, 0x1D, 0x1D), barDuration);
            barAnimation.Children.Add(widthAnimation);

            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTarget(barAnimation, _mainWIndow.BdTimerBar);

            _barAnimation = barAnimation;
            _barAnimation.Begin();
        }
        private void StopAnimations()
        {
            if (_barAnimation == null || _mainWIndow == null)
                return;

            _barAnimation.Stop();
            _mainWIndow.BdTimerBar.Width = _mainWIndow.BdTimerBar.ActualWidth;

            if (_warningAnimations == null) { return; }
            for (int i = 0; i < 3; ++i)
            {
                _warningAnimations[i].Stop();
            }
            _warningAnimations = null;

            SolidColorBrush defaultColor = (SolidColorBrush)FindResource("BackgroundScreenActive");
            _mainWIndow.BdWarning.Background = defaultColor;
            _mainWIndow.BdTimer.Background = defaultColor;
            _mainWIndow.BdCornerLight.Background = defaultColor;
        }
        private void StopGameMedia()
        {
            if (_barAnimation == null || _mainWIndow == null)
                return;
            BdCover.Visibility = Visibility.Visible;
            _mainWIndow.BdTimerBar.Width = 0;

            MainWindow.GameBGM.Stop();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}