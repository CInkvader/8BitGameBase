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

            LoadGame();
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

        public static void ChangeScreen(Page page)
        {
            _frame.Content = page;
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

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void LoadGame()
        {
            CRT.Source = new Uri("Media/CRTEffect.mp4", UriKind.RelativeOrAbsolute);
            CRT.Play();
            await Task.Delay(TimeSpan.FromSeconds(2));

            PlayBlinkAnimation();
            PlayStartup_Animation();
        }
        private static Storyboard InitializeColorAnimation()
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = Color.FromArgb(0xFF, 0x12, 0x12, 0x12),
                To = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(0.1),
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
    }
}