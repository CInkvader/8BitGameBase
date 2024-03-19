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

namespace _8BitGameBase.View.UserControls
{
    public partial class Titlebar : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Window? parentWindow;
        private string _BtnMinimizeSymbol = string.Empty;
        public string BtnMinimizeSymbol
        {
            get { return _BtnMinimizeSymbol; }
            set { _BtnMinimizeSymbol = value; OnPropertyChanged(); }
        }

        public Titlebar()
        {
            DataContext = this;
            Loaded += Titlebar_Loaded;
            InitializeComponent();
            SetTitleBar();
        }

        private void Titlebar_Loaded(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            Maximize();
        }

        private void Close()
        {
            parentWindow?.Close();
        }

        private void Minimize()
        {
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
        }

        private void Maximize()
        {
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Maximized)
                {
                    parentWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    parentWindow.WindowState = WindowState.Maximized;
                }
                SetTitleBar();
            }
        }

        private void SetTitleBar()
        {
            if (parentWindow?.WindowState == WindowState.Maximized)
            {
                GridTitleBar.Background = (SolidColorBrush)FindResource("TitleBarMaximizedColor");
                BtnMinimizeSymbol = "🗗";
            }
            else
            {
                GridTitleBar.Background = (SolidColorBrush)FindResource("TitleBarMinimizedColor");
                BtnMinimizeSymbol = "🗖";
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
