using _8BitGameBase.View.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _8BitGameBase.View.Screens.DifficultySelection;

namespace _8BitGameBase.Backend
{
    class ScoreRecord
    {
        private string _playerName = string.Empty;
        private string _playTimeFormatted = string.Empty;
        private string _difficultyFormatted = string.Empty;
        private int _difficulty = 0;
        private int _round = 0;
        private int _playTime = 0;
        private int _score = 0;

        public string PlayerName { get { return _playerName; } set { _playerName = value; } }
        public string DifficultyFormatted { get { return _difficultyFormatted; } private set { _difficultyFormatted = value; } }
        public string PlaytimeFormatted { get { return _playTimeFormatted; } private set { _playTimeFormatted = value; } }
        public int Difficulty { get { return _difficulty; } set { _difficulty = value; FormatDifficulty(value); } }
        public int HighestRound { get { return _round; } set { _round = value; } }
        public int Playtime { get { return _playTime; } set { _playTime = value; FormatTime(value); } }
        public int Score { get { return _score; } set { _score = value; } }

        private void FormatTime(int playTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(playTime);
            PlaytimeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }
        private void FormatDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 1:
                    DifficultyFormatted = "I";
                    break;
                case 2:
                    DifficultyFormatted = "II";
                    break;
                case 3:
                    DifficultyFormatted = "III";
                    break;
                case 4:
                    DifficultyFormatted = "IV";
                    break;
            }
        }
    }
}
