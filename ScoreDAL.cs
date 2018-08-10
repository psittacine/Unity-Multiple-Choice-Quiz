using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Assets.Scripts.QuizGame
{
    /// <summary>
    /// Placeholder for now.  
    /// </summary>
    public class ScoreDAL : MonoBehaviour {


        private string _dbPath; 

        public void InsertScore(string highScoreName, int score)
        {
            using (var conn = new SqliteConnection(_dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO high_score (name, score) " +
                                      "VALUES (@Name, @Score);";

                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Name",
                        Value = highScoreName
                    });

                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Score",
                        Value = score
                    });

                    var result = cmd.ExecuteNonQuery();
                    Debug.Log("insert score: " + result);
                }
            }
        }

        public void GetHighScores(int limit)
        {
            using (var conn = new SqliteConnection(_dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM high_score ORDER BY score DESC LIMIT @Count;";

                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Count",
                        Value = limit
                    });

                    Debug.Log("scores (begin)");
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var highScoreName = reader.GetString(1);
                        var score = reader.GetInt32(2);
                        var text = string.Format("{0}: {1} [#{2}]", highScoreName, score, id);
                        Debug.Log(text);
                    }
                    Debug.Log("scores (end)");
                }
            }
        }
    }
}
