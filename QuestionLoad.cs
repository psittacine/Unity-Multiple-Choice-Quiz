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
        public AnswerData AnswerData;
        public Dictionary<int, AnswerData> QuestionPool;

        private List<string> _answers;
        private List<bool> _isCorrect;
        private List<int> _numberOfAnswers;
        private string _dbPath;
        

        /// <summary>
        /// Pulls the questions from our chosen categories from the database.
        /// </summary>
        /// <returns>A dictionary with our questions.</returns>
        public Dictionary<int, AnswerData> GetQuestions()
        {
            QuestionPool = new Dictionary<int, AnswerData>();
            _dbPath = "URI=file:" + Application.dataPath + "/StreamingAssets/TEQuizDB.db";
            using (SqliteConnection conn = new SqliteConnection(_dbPath))
            {
                try
                {
                    conn.Open();

                    // First database hit gets the number of answers we can expect for each question.  There's a method to
                    // the madness here.  We need it later and this simplifies that immensely.
                    using (var cmd = conn.CreateCommand())
                    {
                        _numberOfAnswers = new List<int>();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "PRAGMA foreign_keys = 1";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "SELECT count(answer) FROM net_tbl_answers INNER JOIN net_tbl_questions ON net_tbl_answers.question_id = net_tbl_questions.id WHERE net_tbl_questions.category IN (" +
                            RoundData.CategoryParameters + ") GROUP BY net_tbl_answers.question_id ;";
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            _numberOfAnswers.Add(Convert.ToInt32(reader["count(answer)"]));
                        }
                    }

                    // Second hit gets the actual question and answer data.
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "PRAGMA foreign_keys = 1";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT net_tbl_questions.id, question, answer, isCorrect, net_tbl_answers.question_id FROM net_tbl_answers INNER JOIN net_tbl_questions ON net_tbl_answers.question_id = net_tbl_questions.id WHERE net_tbl_questions.category IN (" + RoundData.CategoryParameters + ");";

                        _answers = new List<string>();
                        _isCorrect = new List<bool>();
                        int answerCount = 0;
                        int count = 0;
                        Debug.Log("data (begin)");
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Cycles through all the answers and isCorrect data until it matches the number of
                            // answers we got above.  Then adds it to the AnswerData/Dictionary, resets the variables,
                            // and gets ready for the next question.  If we had a uniform number of answers for each
                            // question then this would likely be unecessary, but who wants a boring, uniform quiz?
                            _answers.Add(Convert.ToString(reader["answer"]));
                            _isCorrect.Add(Convert.ToBoolean(reader["isCorrect"]));
                            answerCount++;

                            if (answerCount == _numberOfAnswers[count])
                            {
                                AnswerData.Question = Convert.ToString(reader["question"]);
                                AnswerData.AnswerPool = _answers;
                                AnswerData.IsCorrect = _isCorrect;
                                QuestionPool.Add(count, AnswerData);
                                _answers = new List<string>();
                                _isCorrect = new List<bool>();
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
                    Debug.Log(ex.Message); 
                }
            }
            return QuestionPool;
        }
    }
}





