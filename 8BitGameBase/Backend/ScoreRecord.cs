using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitGameBase.Backend
{
    class ScoreRecord
    {
        private string _playerName = string.Empty;
        private int _score = 0;

        public string PlayerName { get { return _playerName; } set { _playerName = value; } }
        public int Score { get { return _score; } set { _score = value; } }
    }
}
