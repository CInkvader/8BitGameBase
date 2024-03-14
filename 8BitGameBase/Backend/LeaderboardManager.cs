using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace _8BitGameBase.Backend
{
    static class LeaderboardManager
    {
        private static List<ScoreRecord> _leaderboard = new List<ScoreRecord>();
        private static ScoreRecord? _recordToSave = null;

        private static string _CSVPath = string.Empty;

        public static void InitializeLeaderboard()
        {
            _leaderboard = new List<ScoreRecord>();
            _CSVPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Leaderboard.csv");

            if (File.Exists(_CSVPath))
                RetrieveFromCSV();
            else
                File.Create(_CSVPath);
        }

        public static void AddToLeaderboard(string playerName, int score)
        {
            ScoreRecord newRecord = new ScoreRecord()
            {
                PlayerName = playerName,
                Score = score
            };
            _recordToSave = newRecord;
            SaveLeaderboard();
            _recordToSave = null;

            _leaderboard.Add(newRecord);
            _leaderboard = _leaderboard.OrderBy(ScoreRecord => ScoreRecord.Score).ToList();
        }

        public static List<ScoreRecord> GetTopScores(int count = 10, bool ascending = false)
        {
            List<ScoreRecord> record = new List<ScoreRecord>();

            if (!ascending)
            {
                for (int i = _leaderboard.Count - 1, j = 0; i >= 0 && j < count; --i, ++j)
                {
                    record.Add(new ScoreRecord() { PlayerName = _leaderboard[i].PlayerName, Score = _leaderboard[i].Score });
                }
            }
            else
            {
                for (int i = 0; i < _leaderboard.Count && i < count; ++i)
                {
                    record.Add(new ScoreRecord() { PlayerName = _leaderboard[i].PlayerName, Score = _leaderboard[i].Score });
                }
            }
            return record;
        }

        public static void SaveLeaderboard()
        {
            WriteToCSV();
        }

        private static void RetrieveFromCSV()
        {
            using (TextFieldParser parser = new TextFieldParser(_CSVPath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[]? strings = parser.ReadFields();
                    string[] fields = strings == null ? [] : strings;
                    if (fields == null)
                    {
                        continue;
                    }
                    if (fields.Length != 2)
                    {
                        continue;
                    }

                    ScoreRecord scoreRecord = new ScoreRecord()
                    {
                        PlayerName = fields[0],
                        Score = int.TryParse(fields[1], out int score) ? score : 0
                    };
                    _leaderboard.Add(scoreRecord);
                }
            }
            _leaderboard = _leaderboard.OrderBy(ScoreRecord => ScoreRecord.Score).ToList();
        }

        private static void WriteToCSV()
        {
            if (_recordToSave == null)
            {
                return;
            }
            
            using (StreamWriter writer = new StreamWriter(_CSVPath, true))
            {
                ScoreRecord record = _recordToSave;

                StringBuilder data = new StringBuilder();
                data.Append(record.PlayerName).Append(',');
                data.Append(record.Score);

                writer.WriteLine(data.ToString());
            }
        }
    }
}
