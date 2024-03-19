using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
        private int _bitValue = 0;

        public string BtnContent
        {
            get { return _btnContent; }
            set { _btnContent = value; OnPropertyChanged(); }
        }
        public int BitValue
        {
            get { return _bitValue; }
            set { _bitValue = value; OnPropertyChanged(); }
        }
        public int BtnBitValue
        {
            get { return int.Parse(BtnContent) * BitValue; }
        }

        public void SetBit()
        {
            BtnContent = _btnContent == "0" ? "1" : "0";
        }

        public void PlayValueFadeAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            EasingFunctionBase easingFunction = new PowerEase();
            easingFunction.EasingMode = EasingMode.EaseOut;
            animation.EasingFunction = easingFunction;

            stpDecimalValue.BeginAnimation(OpacityProperty, animation);
        }
        public void StopAnimation()
        {
            stpDecimalValue.BeginAnimation(OpacityProperty, null);
            stpDecimalValue.Opacity = 1;
        }

        private void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
