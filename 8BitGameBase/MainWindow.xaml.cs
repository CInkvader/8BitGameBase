using _8BitGameBase.Backend;
using _8BitGameBase.View.Screens;
using _8BitGameBase.View.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
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
using System.Windows.Media.Animation;

namespace _8BitGameBase
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<Border> _lightEffects = [];

        public event PropertyChangedEventHandler? PropertyChanged;
        private static Frame _frame = new();
        private string _BtnMinimizeSymbol = string.Empty;

        private static MediaPlayer _startupSound = new();
        private static MediaPlayer _menuBGM = new();
        private bool _firstStartup = true;

        public static MainWindow? Instance { get; private set; }
        public string BtnMinimizeSymbol
        {
            get { return _BtnMinimizeSymbol; }
            set { _BtnMinimizeSymbol = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            DataContext = this;
            Instance = this;

            LeaderboardManager.InitializeLeaderboard();
            InitializeComponent();
            InitializeEffects();

            SetTitleBar();
            _frame = MainFrame;

            InitializeGame();
        }
        private void InitializeEffects()
        {
            _lightEffects = new List<Border>()
            {
                BdBackground, BdBackBackground,
                BdDifficulty, BdScore, BdTimer,
                BdWarning, BdCornerLight
            };
        }
        
        private async void InitializeGame()
        {
            InitializeMedia();
            await Task.Delay(TimeSpan.FromSeconds(1));

            _startupSound.Play();
            CRT.Play();

            PlayBlinkAnimation();
            PlayStartup_Animation();
            await Task.Delay(TimeSpan.FromSeconds(6.5));

            _firstStartup = false;
            _startupSound.Stop();
            _startupSound.Close();

            if (MainFrame.Content is MainMenu || MainFrame.Content is Leaderboard)
                _menuBGM.Play();
        }
        private void InitializeMedia()
        {
            CRT.Source = new Uri("Media/Videos/CRTEffect.mp4", UriKind.RelativeOrAbsolute);
            _startupSound.Open(new Uri("Media/Sounds/Effects/StartupSound.mp3", UriKind.RelativeOrAbsolute));
            _menuBGM.Open(new Uri("Media/Sounds/BGM/MainMenu_Jungle.mp3", UriKind.RelativeOrAbsolute));

            _menuBGM.MediaEnded += MenuBGM_MediaEnded;
        }
        private void MenuBGM_MediaEnded(object? sender, EventArgs e)
        {
            _menuBGM.Position = TimeSpan.Zero;
            _menuBGM.Play();
        }

        public static void ChangeScreen(Page page)
        {
            _frame.Content = page;
        }
        public static void StopMenuSoundMedia()
        {
            _startupSound.Stop();
            _startupSound.Close();

            _menuBGM.Stop();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
            SetTitleBar();
        }
        private void SetTitleBar()
        {
            if (WindowState == WindowState.Maximized)
            {
                GridTitleBar.Background = (SolidColorBrush)this.FindResource("TitleBarMaximizedColor");
                BtnMinimizeSymbol = "🗗";
            }
            else
            {
                GridTitleBar.Background = (SolidColorBrush)this.FindResource("TitleBarMinimizedColor");
                BtnMinimizeSymbol = "🗖";
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static Storyboard InitializeColorAnimation()
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = Color.FromArgb(0xFF, 0x12, 0x12, 0x12),
                To = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(0.06),
            };
            DoubleAnimation backgroundOpacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            Storyboard blinkAnimation = new Storyboard();

            blinkAnimation.Children.Add(backgroundOpacityAnimation);
            blinkAnimation.RepeatBehavior = new RepeatBehavior(1);

            blinkAnimation.Children.Add(colorAnimation);
            colorAnimation.RepeatBehavior = new RepeatBehavior(3);

            Storyboard.SetTargetProperty(backgroundOpacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Background.Color"));
            return blinkAnimation;
        }
        private void PlayStartup_Animation()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 0.05,
                Duration = TimeSpan.FromSeconds(1)
            };

            Storyboard.SetTarget(opacityAnimation, CRT);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(opacityAnimation);
            storyboard.Completed += Startup_Animation_Complete;
            storyboard.Begin();
        }
        private void PlayBlinkAnimation()
        {
            foreach (Border element in _lightEffects)
            {
                Storyboard blinkingAnimation = InitializeColorAnimation();

                if (blinkingAnimation == null)
                    continue;

                Storyboard.SetTarget(blinkingAnimation, element);
                blinkingAnimation.Begin();
            }
        }

        private async void Startup_Animation_Complete(object? sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            BdTimerBar.Opacity = 1;
            _frame.Content = new MainMenu();
        }
        private void CRT_MediaEnded(object sender, RoutedEventArgs e)
        {
            CRT.Position = TimeSpan.Zero;
            CRT.Play();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is MainMenu && !_firstStartup)
                _menuBGM.Play();
        }
    }
}