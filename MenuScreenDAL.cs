using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

namespace Assets.Scripts.QuizGame
{

    public class MenuScreenDAL : MonoBehaviour
    {
        public List<int> NumberOfQuestions;
        private string dbPath;

        public List<int> GetNumberOfQuestions()
        {
            NumberOfQuestions = new List<int>();
            dbPath = "URI=file:" + Application.dataPath + "/SQLLite/TEQuizDB.db";
            using (SqliteConnection conn = new SqliteConnection(dbPath))
            {
                try
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {

                       cmd.CommandText =
                            "SELECT count(question) FROM net_tbl_questions GROUP BY net_tbl_questions.category;";
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            NumberOfQuestions.Add(Convert.ToInt32(reader["count(question)"]));
                        }
                    }

                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }

            }

            return NumberOfQuestions;
        }


    }
}