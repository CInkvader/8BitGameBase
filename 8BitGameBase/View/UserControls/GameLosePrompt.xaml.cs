using _8BitGameBase.View.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class GameLosePrompt : UserControl
    {
        public GameLosePrompt()
        {
            InitializeComponent();
        }

        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainGame());
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeScreen(new MainMenu());
        }
    }
}
