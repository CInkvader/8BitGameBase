using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace _8BitGameBase.View.UserControls
{
    public partial class SliderControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private static readonly Uri _volumeEnabledPath = new("pack://application:,,,/Media/Images/VolumeEnabled.png");
        private static readonly Uri _volumeDisabledPath = new("pack://application:,,,/Media/Images/VolumeDisabled.png");

        private static Uri? _volumeIconSource = null;
        public Uri? VolumeIconSource
        {
            get { return _volumeIconSource; }
            set { _volumeIconSource = value; OnPropertyChanged(); }
        }

        private bool _isVolumeEnabled = true;
        private double _previousVolume = 0.5;

        private string _sliderText = string.Empty;
        public string SliderText
        {
            get { return _sliderText; }
            set { _sliderText = value; OnPropertyChanged(); }
        }

        private double _sliderValue = 0.5;
        public double SliderValue
        {
            get { return _sliderValue; }
            set { _sliderValue = value; CheckValue(value); OnPropertyChanged(); }
        }

        public SliderControl()
        {
            DataContext = this;
            VolumeIconSource = _volumeEnabledPath;

            InitializeComponent();
        }
        private void CheckValue(double value)
        {
            if (value == 0)
                _isVolumeEnabled = false;
            else
            {
                _isVolumeEnabled = true;
                _previousVolume = value;
            }

            VolumeIconSource = _isVolumeEnabled ? _volumeEnabledPath : _volumeDisabledPath;
        }
        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            _isVolumeEnabled = !_isVolumeEnabled;

            if (!_isVolumeEnabled)
            {
                _previousVolume = SliderValue;
                SliderValue = 0;
            }
            else
            {
                SliderValue = _previousVolume;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
