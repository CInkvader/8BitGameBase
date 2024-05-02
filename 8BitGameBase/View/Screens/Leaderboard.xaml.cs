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
    public partial class Leaderboard : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private object? _previousPage = null;
        private int _difficultyView = 0;

        private string _difficultyText = string.Empty;

        public string DifficultyText
        {
            get { return _difficultyText; }
            set { _difficultyText = value; OnPropertyChanged(); }
        }

        public Leaderboard(object data)
        {
            DataContext = this;
            _previousPage = data ?? new MainMenu();
            InitializeComponent();

            lvLeaderboard.ItemContainerStyle = (Style)FindResource("lvItemLeaderboard");

            _difficultyView = 1; // 1 - Easy, 2 - Normal, 3 - Hard, 4 - Extreme
            InitializeHeader();
            UpdatePage();
        }

        private void InitializeHeader()
        {
            string[] headers = { "Rank", "Name", "Round", "Play Time", "Score" };
            float[] gridScale = { 1f, 3f, 2f, 2f, 2f };

            for (int i = 0; i < 5; ++i)
            {
                GridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(gridScale[i], GridUnitType.Star) });
                TextBlock textBlock = AddTextBlock(headers[i]);
                textBlock.Margin = new Thickness(0,10,0,0);

                GridHeader.Children.Add(textBlock);
                Grid.SetColumn(textBlock, i);
            }
        }

        private void RetrieveLeaderboard(int difficultyFilter)
        {
            int i = 1;
            foreach (ScoreRecord record in LeaderboardManager.GetTopScores(difficultyFilter))
            {
                float[] gridScale = { 1f, 3f, 2f, 2f, 2f };
                ListViewItem item = new();
                Grid grid = new();
                grid.Width = 700;

                TextBlock rank = AddTextBlock((i++).ToString());
                TextBlock name = AddTextBlock(record.PlayerName);
                TextBlock round = AddTextBlock(record.HighestRound.ToString());
                TextBlock playtime = AddTextBlock(record.PlaytimeFormatted);
                TextBlock score = AddTextBlock(record.Score.ToString());
                TextBlock[] textBlocks = { rank, name, round, playtime, score };

                for (int j = 0; j < 5; ++j)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(gridScale[j], GridUnitType.Star) });
                    grid.Children.Add(textBlocks[j]);
                    Grid.SetColumn(textBlocks[j], j);
                }

                item.Content = grid;
                lvLeaderboard.Items.Add(item);
            }
        }

        private TextBlock AddTextBlock(string text)
        {
            TextBlock textblock = new()
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = (FontFamily)FindResource("ArcadeFont"),
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 20
            };

            return textblock;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            if (_previousPage != null)
                MainWindow.ChangeScreen((Page)(_previousPage ?? new MainMenu()));
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            if (_difficultyView > 0)
                --_difficultyView;
            UpdatePage();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.PlayButtonSound();
            if (_difficultyView < 4)
                ++_difficultyView;
            UpdatePage();
        }

        private void UpdatePage()
        {
            lvLeaderboard.Items.Clear();
            RetrieveLeaderboard(_difficultyView);
            switch (_difficultyView)
            {
                case 1:
                    DifficultyText = "EASY";
                    BtnPrevious.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    DifficultyText = "NORMAL";
                    BtnPrevious.Visibility = Visibility.Visible;
                    break;
                case 3:
                    DifficultyText = "HARD";
                    BtnNext.Visibility = Visibility.Visible;
                    break;
                case 4:
                    DifficultyText = "EXTREME";
                    BtnNext.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
