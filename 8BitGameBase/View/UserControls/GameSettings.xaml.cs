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
    public partial class GameSettings : UserControl
    {
        private List<SliderControl> _volumeSliders = new();
        private double _masterVolume = 0.5;
        private static double[] _sliderValues = [];

        public GameSettings()
        {
            if (_sliderValues.Length == 0)
            {
                _sliderValues = [0.5, 0.5, 0.5, 0.5];
            }
            InitializeComponent();

            _volumeSliders.Add(sldMasterVolume);
            _volumeSliders.Add(sldMusicVolume);
            _volumeSliders.Add(sldEffectsVolume);
            _volumeSliders.Add(sldUIVolume);

            int i = 0;
            foreach (SliderControl slider in _volumeSliders)
            {
                slider.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
                slider.VolumeSlider.Tag = i.ToString();
                slider.VolumeSlider.Value = _sliderValues[i];
                ++i;
            }

            sldMasterVolume.SliderText = "MASTER  VOLUME";
            sldMusicVolume.SliderText = "MUSIC  VOLUME";
            sldEffectsVolume.SliderText = "EFFECTS  VOLUME";
            sldUIVolume.SliderText = "UI  VOLUME";

            SetVolume();
        }
        public static void SetSliderValues(double[] values)
        {
            if (values.Length != 4)
            {
                return;
            }
            _sliderValues = values;
        }
        public static double[] GetSliderValues()
        {
            return _sliderValues;
        }

        private void SetVolume()
        {
            MusicVolume_Change();
            EffectsVolume_Change();
            UIVolume_Change();
        }
        private void MasterVolume_Change()
        {
            _masterVolume = sldMasterVolume.SliderValue;

            MusicVolume_Change();
            EffectsVolume_Change();
            UIVolume_Change();
        }
        private void MusicVolume_Change()
        {
            MainWindow.GameBGM.Volume = sldMusicVolume.SliderValue * _masterVolume;
            MainWindow.MenuBGM.Volume = sldMusicVolume.SliderValue * _masterVolume;
        }
        private void EffectsVolume_Change()
        {
            MainWindow.GameLoseSound.Volume = sldEffectsVolume.SliderValue * _masterVolume;
            MainWindow.GameLoseFallSound.Volume = sldEffectsVolume.SliderValue * _masterVolume;
            MainWindow.GameCorrectAnswer.Volume = sldEffectsVolume.SliderValue * _masterVolume;
        }
        private void UIVolume_Change()
        {
            foreach (MediaPlayer buttonSound in MainWindow.ButtonSounds)
            {
                buttonSound.Volume = sldUIVolume.SliderValue * _masterVolume;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            switch (slider.Tag)
            {
                case "0":
                    _sliderValues[0] = _volumeSliders[0].SliderValue;
                    MasterVolume_Change();
                    break;
                case "1":
                    _sliderValues[1] = _volumeSliders[1].SliderValue;
                    MusicVolume_Change();
                    break;
                case "2":
                    _sliderValues[2] = _volumeSliders[2].SliderValue;
                    EffectsVolume_Change();
                    break;
                case "3":
                    _sliderValues[3] = _volumeSliders[3].SliderValue;
                    UIVolume_Change();
                    break;
            }
        }
    }
}
