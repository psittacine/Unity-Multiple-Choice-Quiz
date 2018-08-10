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

        // This is the List that allows us to show the number of questions for each category as the user
        // selects them on the Menu Screen.
        private List<int> _numberOfQuestions;
        private string _dbPath;

        /// <summary>
        /// Gets the number of questions for each category from the database.
        /// </summary>
        /// <returns>A List of ints that correspond to each category's total questions.</returns>
        public List<int> GetNumberOfQuestions()
        {
            _numberOfQuestions = new List<int>();
            _dbPath = "URI=file:" + Application.dataPath + "/StreamingAssets/TEQuizDB.db";
            using (SqliteConnection conn = new SqliteConnection(_dbPath))
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
                            _numberOfQuestions.Add(Convert.ToInt32(reader["count(question)"]));
                        } 
                    }

                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
            return _numberOfQuestions;
        }
    }
}