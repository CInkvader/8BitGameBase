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

namespace _8BitGameBase.View.UserControls
{
    public partial class SaveRecordPrompt : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _nameInput = string.Empty;

        public string NameInput
        {
            get { return _nameInput; }
            set { _nameInput = value; OnPropertyChanged(); }
        }

        public SaveRecordPrompt()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameInput.Length >= 15)
            {
                NameInput = NameInput.Substring(0, 15);
                ((TextBox)sender).CaretIndex = NameInput.Length;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void NameEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char input = e.Text.ToUpper()[0];
            if (!(input >= 65 && input <= 90))
            {
                e.Handled = true;
            }
        }
    }
}
