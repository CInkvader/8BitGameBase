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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8BitGameBase.View.Screens
{
    public partial class DifficultySelection : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Button? _selectedButton = null;
        private readonly object? _previousPage = null;

        private string _tbMultiplier = string.Empty;

        public string TbMultiplier
        {
            get { return _tbMultiplier; }
            set { _tbMultiplier = value; OnPropertyChanged(); }
        }
        public enum GameDifficulty
        {
            Easy,
            Normal,
            Hard,
            Extreme
        }

        public DifficultySelection(object previousPage)
        {
            _previousPage = previousPage ?? new Menu();
            DataContext = this;
            TbMultiplier = "0x";
            InitializeComponent();
        }

        private void BtnDiffculty_Click(object sender, RoutedEventArgs e)
        {
            if ((Button)sender != _selectedButton)
            {
                BtnBitResizeAnimation(_selectedButton ?? (Button)sender, [60, 50], [210, 200]);
                _selectedButton = (Button)sender;

                string difficulty = (string)_selectedButton.Tag;
                TbMultiplier = difficulty + 'x';
                BtnBitResizeAnimation(_selectedButton, [50, 60], [200, 210]);

                return;
            }
            MainWindow.ChangeScreen(new MainGame(_previousPage ?? new MainMenu(), (GameDifficulty)int.Parse((string)_selectedButton.Tag)));
        }
        private static void BtnBitResizeAnimation(Button? button, int[] fromToHeight, int[] fromToWidth)
        {
            if (fromToHeight.Length != 2 && fromToWidth.Length != 2 && button != null)
            {
                return;
            }

            DoubleAnimation heightAnimation = new()
            {
                From = fromToHeight[0],
                To = fromToHeight[1],
                Duration = TimeSpan.FromSeconds(0)
            };
            DoubleAnimation widthAnimation = new()
            {
                From = fromToWidth[0],
                To = fromToWidth[1],
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
        private void BtnDifficultyBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen((Page)(_previousPage ?? new MainMenu()));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
