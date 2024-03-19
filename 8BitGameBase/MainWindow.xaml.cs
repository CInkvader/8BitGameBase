using _8BitGameBase.Backend;
using _8BitGameBase.View.Screens;
using _8BitGameBase.View.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.IO;
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

        private readonly static MediaPlayer _startupSound = new();
        private static MediaPlayer _menuBGM = new();
        
        private static MediaPlayer _gameBGM = new();
        private static MediaPlayer _gameCorrectAnswer = new();
        private static MediaPlayer _gameLoseSound = new();
        private static MediaPlayer _gameLoseFallSound = new();
        private static Uri? _buttonClickSoundPath = null;
        private static List<MediaPlayer> _buttonSounds = [];

        private string _volumeSettingsPath = string.Empty;
        private bool _firstStartup = true;

        public static MediaPlayer StartupSound { get { return _startupSound; } }
        public static MediaPlayer MenuBGM { get { return _menuBGM; } private set { _menuBGM = value; } }
        public static MediaPlayer GameBGM { get { return _gameBGM; } private set { _gameBGM = value; } }
        public static MediaPlayer GameCorrectAnswer { get { return _gameCorrectAnswer; } private set { _gameCorrectAnswer = value; } }
        public static MediaPlayer GameLoseSound { get { return _gameLoseSound; } private set { _gameLoseSound = value; } }
        public static MediaPlayer GameLoseFallSound {  get { return _gameLoseFallSound; } private set { _gameLoseFallSound = value; } }
        public static List<MediaPlayer> ButtonSounds { get { return _buttonSounds; } }
        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            DataContext = this;
            Instance = this;
            _volumeSettingsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VolumePreferences.txt");
            RetrieveSoundSettings();

            LeaderboardManager.InitializeLeaderboard();
            InitializeComponent();
            InitializeBorderEffects();
            _frame = MainFrame;

            InitializeGame();
        }
        private async void InitializeGame()
        {
            InitializeGameMedia();
            InitializeStartupMedia();
            await Task.Delay(TimeSpan.FromSeconds(2));

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
        private void InitializeBorderEffects()
        {
            _lightEffects = new List<Border>()
            {
                BdBackground, BdBackBackground,
                BdDifficulty, BdScore, BdTimer,
                BdWarning, BdCornerLight
            };
        }
        private void InitializeStartupMedia()
        {
            CRT.Source = new Uri("Media/Videos/CRTEffect.mp4", UriKind.RelativeOrAbsolute);
            _startupSound.Open(new Uri("Media/Sounds/Effects/StartupSound.mp3", UriKind.RelativeOrAbsolute));
        }
        private static void InitializeGameMedia()
        {
            _menuBGM.Open(new Uri("Media/Sounds/BGM/MainMenu_Jungle.mp3", UriKind.RelativeOrAbsolute));
            _menuBGM.MediaEnded += BGM_MediaEnded;

            _buttonClickSoundPath = new Uri("Media/Sounds/Effects/ButtonPress_3.mp3", UriKind.RelativeOrAbsolute);
            _buttonSounds = new List<MediaPlayer>();

            _gameCorrectAnswer.MediaEnded += Sound_MediaEnded_Reset;
            _gameLoseSound.MediaEnded += Sound_MediaEnded_Reset;
            _gameLoseFallSound.MediaEnded += Sound_MediaEnded_Reset;
            _gameBGM.MediaEnded += BGM_MediaEnded;

            _gameCorrectAnswer.Open(new Uri("Media/Sounds/Effects/CorrectAnswer.mp3", UriKind.RelativeOrAbsolute));
            _gameLoseSound.Open(new Uri("Media/Sounds/Effects/GameLose.mp3", UriKind.RelativeOrAbsolute));
            _gameLoseFallSound.Open(new Uri("Media/Sounds/Effects/GameLoseFall.mp3", UriKind.RelativeOrAbsolute));

            for (int i = 0; i < 10; ++i)
            {
                _buttonSounds.Add(new MediaPlayer());

                _buttonSounds[i].Open(_buttonClickSoundPath);
                _buttonSounds[i].MediaEnded += Sound_MediaEnded;
            }
        }

        public static void PlayButtonSound()
        {
            _buttonSounds[0].Play();
            double volume = _buttonSounds[0].Volume;
            _buttonSounds.RemoveAt(0);

            MediaPlayer newSound = new MediaPlayer();
            newSound.Open(_buttonClickSoundPath);
            newSound.Volume = volume;
            newSound.MediaEnded += Sound_MediaEnded;
            _buttonSounds.Add(newSound);
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StopMenuSoundMedia();
            SaveSoundSettings();
            ClearSoundMedia();
            Application.Current.Shutdown();
        }
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is MainMenu && !_firstStartup)
            {
                MenuBGM.Play();
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private static void Sound_MediaEnded(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            ((MediaPlayer)sender).Close();
        }
        private static void Sound_MediaEnded_Reset(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            ((MediaPlayer)sender).Stop();
            ((MediaPlayer)sender).Position = TimeSpan.Zero;
        }
        private static async void BGM_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer? media = ((MediaPlayer?)sender);
            if (media == null)
                return;
            media.Position = TimeSpan.Zero;
            await Task.Delay(TimeSpan.FromSeconds(3));
            media.Play();
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

        private void ClearSoundMedia()
        {
            CRT.Close();
            MenuBGM.Close();
            GameBGM.Close();
            GameLoseSound.Close();
            GameLoseFallSound.Close();
            GameCorrectAnswer.Close();

            foreach (MediaPlayer media in _buttonSounds)
            {
                media.Close();
            }
            _buttonSounds.Clear();
        }
        private void RetrieveSoundSettings()
        {
            double[] values = [0.5, 0.5, 0.5, 0.5];
            if (!File.Exists(_volumeSettingsPath))
            {
                File.Create(_volumeSettingsPath);
                GameSettings.SetSliderValues(values);
                return;
            }

            string fileContent = File.ReadAllText(_volumeSettingsPath);
            string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int i = 0;
            foreach (string line in lines)
            {
                double value = double.TryParse(line, out double result) ? result : 0.5;
                values[i] = value;

                if (i++ == 3)
                    break;
            }
            GameSettings.SetSliderValues(values);
            StartupSound.Volume = values[0];
        }
        private void SaveSoundSettings()
        {
            double[] values = GameSettings.GetSliderValues();
            string data = string.Empty;

            foreach (double value in values)
            {
                data += value + "\n";
            }
            File.WriteAllText(_volumeSettingsPath, data);
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
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}