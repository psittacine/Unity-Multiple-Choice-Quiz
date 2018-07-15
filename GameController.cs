using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using NUnit.Framework.Api;
using Random = UnityEngine.Random;

namespace Assets.Scripts.QuizGame
{
    public class GameController : MonoBehaviour
    {

        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI[] answerTexts;
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI timeDisplay;
        private GameObject dataController;
        private RoundData currentRoundData;
        public Dictionary<int, AnswerData> questionPool;
        public GameObject questionDisplay;
        public GameObject roundEndDisplay;


        private List<string> correctAnswer;
        private bool isRoundActive;
        private float timeRemaining;
        private int questionIndex;
        private int playerScore;


        
        void Start()
        {

            DataController dataController = GameObject.FindGameObjectWithTag("DataController").GetComponent<DataController>();
            currentRoundData = dataController.GetCurrentRoundData();
            QuestionLoad ql = gameObject.GetComponent<QuestionLoad>();
            questionPool = ql.GetQuestions();
            timeRemaining = currentRoundData.timeLimitInSeconds;

            playerScore = 0;
            questionIndex = 0;
            ShowQuestion();
            isRoundActive = true;
            UpDateTimeRemainingDisplay();
        }

        private void ShowQuestion()
        {
            // Prints the current question.
            questionText.text = questionPool[questionIndex].Question;
            // We're going to mix up our answers here.  Easiest way I found to do this (and still keep
            // track of whether the answer is correct without creating another class) was to mix up 
            // the text boxes instead of the answers themselves.  That way the isCorrect and answers still match up.
            // Likely a better way of doing it, but...  ¯\_(ツ)_/¯
            var rand = new System.Random();
            // Scrambles the order of the text boxes and assigns them to the new list.
            var randomList = answerTexts.OrderBy(x => rand.Next()).ToList();

            // Simple for loop to print all our answers, no matter how many there are.
            for (int i = 0; i < questionPool[questionIndex].AnswerPool.Count; i++)
            {
                randomList[i].text = questionPool[questionIndex].AnswerPool[i];
                // Checks to see if the answer currently being printed is a correct one.
                if (questionPool[questionIndex].isCorrect[i])
                {
                    // If so, adds the button tag to our correct answer list.
                    correctAnswer.Add(randomList[i].tag);
                }
            }

        }

        public void AnswerButtonClicked(string buttonTag)
        {
            Debug.Log("Button Clicked");
            

            if (correctAnswer.Contains(buttonTag))
            {
                playerScore += currentRoundData.pointsAddedForCorrectAnswer;
                scoreText.text = "Score: " + playerScore;
                if (questionPool.Keys.Count > questionIndex + 1)
                {
                    questionIndex++;
                    ShowQuestion();
                }
                else
                {
                    EndRound();
                }
            }
        }

        public void EndRound()
        {
            isRoundActive = false;
            questionDisplay.SetActive(false);
            roundEndDisplay.SetActive(true);
        }

        public void ReturnToMenu()
        {
            SceneManager.LoadScene("MenuScreen");
        }

        private void UpDateTimeRemainingDisplay()
        {
            timeDisplay.text = "Time: " + Mathf.Round(timeRemaining);
        }

        // Update is called once per frame
        void Update()
        {

            if (isRoundActive)
            {

                timeRemaining -= Time.deltaTime;
                UpDateTimeRemainingDisplay();

                if (timeRemaining <= 0f)
                {
                    EndRound();
                }


            }



        }
        
    }
}
