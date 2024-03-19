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
    public partial class ConfirmationPrompt : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = null;

        private string _promptDescription = string.Empty;
        private string _firstOptionContent = string.Empty;
        private string _secondOptionContent = string.Empty;
        private string _thirdOptionContent = string.Empty;

        public string PromptDescription 
        { 
            get { return _promptDescription; }
            set { _promptDescription = value; OnPropertyChanged(); }
        }
        public string FirstOptionContent
        {
            get { return _firstOptionContent; }
            set { _firstOptionContent = value; OnPropertyChanged(); }
        }
        public string SecondOptionContent
        {
            get { return _secondOptionContent; }
            set { _secondOptionContent = value; OnPropertyChanged(); }
        }
        public string ThirdOptionContent
        {
            get { return _thirdOptionContent; }
            set { _thirdOptionContent = value; OnPropertyChanged(); }
        }

        public ConfirmationPrompt()
        {
            DataContext = this;
            InitializeComponent();
        }
        
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
