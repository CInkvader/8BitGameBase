using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace _8BitGameBase.View.UserControls
{
    public partial class PlayButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public PlayButton()
        {
            DataContext = this;
            _btnContent = string.Empty;
            InitializeComponent();
        }

        private string _btnContent = string.Empty;

        public string BtnContent
        {
            get { return _btnContent; }
            set { _btnContent = value; OnPropertyChanged(); }
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
