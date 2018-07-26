using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

namespace Assets.Scripts.QuizGame
{
    public class QuestionLoad : MonoBehaviour
    {
        public List<string> questions;
        public List<string> Answers;
        public List<bool> IsCorrect;
        public AnswerData AnswerData;
        public Dictionary<int, AnswerData> QuestionPool;
        private List<int> numberOfAnswers;
        private string dbPath;
        


        private void Start()
        {
            
        }

        public Dictionary<int, AnswerData> GetQuestions()
        {
            QuestionPool = new Dictionary<int, AnswerData>();
            dbPath = "URI=file:" + Application.dataPath + "/SQLLite/TEQuizDB.db";
            using (SqliteConnection conn = new SqliteConnection(dbPath))
            {
                try
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        numberOfAnswers = new List<int>();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "PRAGMA foreign_keys = 1";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "SELECT count(answer) FROM net_tbl_answers INNER JOIN net_tbl_questions ON net_tbl_answers.question_id = net_tbl_questions.id WHERE net_tbl_questions.category IN (" +
                            RoundData.categoryParameters + ") GROUP BY net_tbl_answers.question_id ;";
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            numberOfAnswers.Add(Convert.ToInt32(reader["count(answer)"]));
                        }
                    }

                    using (var cmd = conn.CreateCommand())
                    { 
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "PRAGMA foreign_keys = 1";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT net_tbl_questions.id, question, answer, isCorrect, net_tbl_answers.question_id FROM net_tbl_answers INNER JOIN net_tbl_questions ON net_tbl_answers.question_id = net_tbl_questions.id WHERE net_tbl_questions.category IN (" + RoundData.categoryParameters + ");";

                        Answers = new List<string>();
                        IsCorrect = new List<bool>();
                        int answerCount = 0;
                        int count = 0;
                        Debug.Log("data (begin)");
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            
                            Answers.Add(Convert.ToString(reader["answer"]));
                            IsCorrect.Add(Convert.ToBoolean(reader["isCorrect"]));
                            answerCount++;

                            if (answerCount == numberOfAnswers[count])
                            {
                                AnswerData.Question = Convert.ToString(reader["question"]);
                                AnswerData.AnswerPool = Answers;
                                AnswerData.isCorrect = IsCorrect;
                                QuestionPool.Add(count, AnswerData);
                                Answers = new List<string>();
                                IsCorrect = new List<bool>();
                                AnswerData = new AnswerData();
                                count++;
                                answerCount = 0;
                            }
                        }
                        
                        Debug.Log("data (end)");
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }



            return QuestionPool;



        }


        


        public void InsertScore(string highScoreName, int score)
        {
            using (var conn = new SqliteConnection(dbPath))
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
            using (var conn = new SqliteConnection(dbPath))
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





