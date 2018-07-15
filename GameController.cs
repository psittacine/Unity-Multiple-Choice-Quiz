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
            // Finds the data controller to pull all the current round information such as time limit, amount of score, etc.
            DataController dataController = GameObject.FindGameObjectWithTag("DataController").GetComponent<DataController>();
            currentRoundData = dataController.GetCurrentRoundData();
            // Instantiates the question load script.
            QuestionLoad ql = gameObject.GetComponent<QuestionLoad>();
            // Grabs the questions from the DB via the GetQuestions method.
            questionPool = ql.GetQuestions();
            // Assigns the time from the data controller.
            timeRemaining = currentRoundData.timeLimitInSeconds;

            // Here's where things start happening.  Resets the variables and calls the method to 
            // start showing questions.  Also flips the flag to make the round active and starts
            // the timer.
            playerScore = 0;
            questionIndex = 0;
            ShowQuestion();
            isRoundActive = true;
            UpDateTimeRemainingDisplay();
        }

        /// <summary>
        /// Cycles through the questions/answers in the dictionary and prints them on the screen.
        /// </summary>
        private void ShowQuestion()
        {
            // Instantiates the list as we'll use it below.
            correctAnswer = new List<string>();
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
                // Sets the gameobject to active so it can be printed to.  I've turned them off because if there
                // are only two answers to be printed, we don't want the user to be able to click on invisible buttons 3 and 4.
                randomList[i].gameObject.SetActive(true);
                // Prints the answer to our scrambled text box.
                randomList[i].text = questionPool[questionIndex].AnswerPool[i];
                // Checks to see if the answer currently being printed is a correct one.
                if (questionPool[questionIndex].isCorrect[i])
                {
                    // If so, adds the button tag to our correct answer list.
                    correctAnswer.Add(randomList[i].tag);
                }
            }

        }

        /// <summary>
        /// Checks the button clicked to see if it was a right or wrong choice.
        /// </summary>
        /// <param name="buttonTag">Passes the button tag</param>
        public void AnswerButtonClicked(string buttonTag)
        {
            Debug.Log("Button Clicked");
            // Checks the button that was clicked to see if the tag matches a correct answer
            if (correctAnswer.Contains(buttonTag))
            {
                // If it matches, it removes that button from our list, to prevent it being clicked
                // again if there's more than one correct answer.
                correctAnswer.Remove(buttonTag);
                // Here's where we check to see if it's the only correct answer or not.  If it is, it
                // gives the points to the player and then updates the score on screen.
                if (correctAnswer.Count == 0)
                {
                    playerScore += currentRoundData.pointsAddedForCorrectAnswer;
                    scoreText.text = "Score: " + playerScore;
                    // Checks to see if there are any more questions after this one.  If so, it increases
                    // the question index and moves to the next question.
                    if (questionPool.Keys.Count > questionIndex + 1)
                    {
                        questionIndex++;
                        ShowQuestion();
                    }
                    else
                    {
                        // No more questions? End of round.
                        EndRound();
                    }
                }
                else
                {
                    // If there's more than one correct answer, but they clicked on one of them, it gives the player 5 points for
                    // getting a right answer (it's arbitrary, just wanted to see something happen) and then waits
                    // for them to click another answer. This entire section is going to be rewritten to allow a submit button
                    // to be added.  Much of the logic is going to remain the same; so figured I'd tackle it now and then figure out
                    // answer highlighting and such later on.
                    playerScore += 5;
                    scoreText.text = "Score: " + playerScore;
                }
            }
            else
            {
                // If they clicked on a wrong button, it takes 5 points away.  Again.. totally arbitrary and will be rewritten soon.
                playerScore -= 5;
                scoreText.text = "Score: " + playerScore;
            }
        }
        
        /// <summary>
        /// Ends the round.
        /// </summary>
        public void EndRound()
        {
            // Swaps the flag to false
            isRoundActive = false;
            // Turns off the quiz screen
            questionDisplay.SetActive(false);
            // Shows the game over screen.
            roundEndDisplay.SetActive(true);
        }

        /// <summary>
        /// Brings us back to the Menu Screen Scene.. rhymes!
        /// </summary>
        public void ReturnToMenu()
        {
            SceneManager.LoadScene("MenuScreen");
        }

        /// <summary>
        /// Updates the time text.
        /// </summary>
        private void UpDateTimeRemainingDisplay()
        {
            timeDisplay.text = "Time: " + Mathf.Round(timeRemaining);
        }

        // Update is called once per frame
        void Update()
        {
            // Checks the flag to see if a round is currently underway.
            if (isRoundActive)
            {
                // Counts down the time from the time remaining variable.
                timeRemaining -= Time.deltaTime;
                UpDateTimeRemainingDisplay();
                // Checks to see if time ran out.
                if (timeRemaining <= 0f)
                {
                    EndRound();
                }


            }
            
        }
        
    }
}
