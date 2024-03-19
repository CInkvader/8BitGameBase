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

        public static void AddToLeaderboard(ScoreRecord newRecord)
        {
            WriteToCSV(newRecord);
            _leaderboard.Add(newRecord);
            _leaderboard = _leaderboard.OrderBy(ScoreRecord => ScoreRecord.Score).ToList();
        }

        public static List<ScoreRecord> GetTopScores(int difficulty, int count = 10, bool ascending = false)
        {
            List<ScoreRecord> record = new List<ScoreRecord>();

            if (!ascending)
            {
                int i = _leaderboard.Count - 1;
                int j = 0;
                while (!ascending && (i >= 0 && j < count))
                {
                    if (_leaderboard[i].Difficulty == difficulty)
                    {
                        record.Add(new ScoreRecord()
                        {
                            PlayerName = _leaderboard[i].PlayerName,
                            Score = _leaderboard[i].Score,
                            Difficulty = _leaderboard[i].Difficulty,
                            Playtime = _leaderboard[i].Playtime,
                            HighestRound = _leaderboard[i].HighestRound
                        });
                        ++j;
                    }
                    --i;
                }
            }
            else
            {
                int i = 0;
                int j = 0;

                while (i < _leaderboard.Count && j < count)
                {
                    if (_leaderboard[i].Difficulty == difficulty)
                    {
                        record.Add(new ScoreRecord()
                        {
                            PlayerName = _leaderboard[i].PlayerName,
                            Score = _leaderboard[i].Score,
                            Difficulty = _leaderboard[i].Difficulty,
                            Playtime = _leaderboard[i].Playtime,
                            HighestRound = _leaderboard[i].HighestRound
                        });

                        ++j;
                    }
                    ++i;
                }
            }
            return record;
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
                    if (fields.Length != 5)
                    {
                        continue;
                    }

                    ScoreRecord scoreRecord = new ScoreRecord()
                    {
                        PlayerName = fields[0],
                        Score = int.TryParse(fields[1], out int score) ? score : 0,
                        Difficulty = int.TryParse(fields[2], out int difficulty) ? difficulty : 0,
                        Playtime = int.TryParse(fields[3], out int playtime) ? playtime : 0,
                        HighestRound = int.TryParse(fields[4], out int highestRound) ? highestRound : 0
                    };
                    _leaderboard.Add(scoreRecord);
                }
            }
            _leaderboard = _leaderboard.OrderBy(ScoreRecord => ScoreRecord.Score).ToList();
        }
        
        private static void WriteToCSV(ScoreRecord recordToSave)
        {
            using (StreamWriter writer = new StreamWriter(_CSVPath, true))
            {
                ScoreRecord record = recordToSave;

                StringBuilder data = new StringBuilder();
                data.Append(record.PlayerName).Append(',');
                data.Append(record.Score).Append(',');
                data.Append(record.Difficulty).Append(',');
                data.Append(record.Playtime).Append(',');
                data.Append(record.HighestRound);

                writer.WriteLine(data.ToString());
            }
        }
    }
}
