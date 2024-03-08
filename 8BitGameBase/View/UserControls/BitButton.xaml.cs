using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace _8BitGameBase.View.UserControls
{
    public partial class BitButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BitButton()
        {
            DataContext = this;
            _btnContent = "0";
            InitializeComponent();
        }

        private string _btnContent = "Play Game";
        private int _btnTag = 0;

        public string BtnContent
        {
            get { return _btnContent; }
            set { _btnContent = value; OnPropertyChanged(); }
        }
        public int BtnTag
        {
            get { return _btnTag; }
            set { _btnTag = value; OnPropertyChanged(); }
        }
        public int BtnBitValue
        {
            get { return int.Parse(BtnContent) * BtnTag; }
        }

        public void SetBit()
        {
            BtnContent = _btnContent == "0" ? "1" : "0";
        }
        private void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
