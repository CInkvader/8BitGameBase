using _8BitGameBase.Backend;
using System;
using System.Collections.Generic;
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
    public partial class Leaderboard : Page
    {
        private object? _previousPage = null;
        public Leaderboard(object data)
        {
            _previousPage = data ?? new MainMenu();
            InitializeComponent();

            lvLeaderboard.ItemContainerStyle = (Style)this.FindResource("lvItemLeaderboard");
            RetrieveLeaderboard();
        }

        private void RetrieveLeaderboard()
        {
            int i = 1;
            foreach (ScoreRecord record in LeaderboardManager.GetTopScores())
            {
                ListViewItem item = new();
                Grid grid = new();
                grid.Width = 670;
                grid.Margin = new Thickness(20,0,20,0);

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                TextBlock rank = new()
                {
                    Text = (i++).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontFamily = (FontFamily)this.FindResource("ArcadeFont")
                };
                TextBlock name = new()
                {
                    Text = record.PlayerName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontFamily = (FontFamily)this.FindResource("ArcadeFont")
                };
                TextBlock score = new()
                {
                    Text = record.Score.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontFamily = (FontFamily)this.FindResource("ArcadeFont")
                };
                
                grid.Children.Add(rank);
                grid.Children.Add(name);
                grid.Children.Add(score);

                Grid.SetColumn(rank, 0);
                Grid.SetColumn(name, 1);
                Grid.SetColumn(score, 2);

                item.Content = grid;
                lvLeaderboard.Items.Add(item);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_previousPage != null)
                MainWindow.ChangeScreen((Page)(_previousPage ?? new MainMenu()));
        }
    }
}
