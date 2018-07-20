using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using NUnit.Framework.Api;
using UnityEngine.UI;
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
        public Dictionary<int, AnswerData> questionPool;
        public GameObject questionDisplay;
        public GameObject roundEndDisplay;

        private List<string> correctAnswers;
        private List<string> selectedAnswers;
        private int questionIndex;
        private int numberOfQuestions;
        private int playerScore;
        private Color selectedColor;
        private Color defaultColor;
        private bool isRoundActive;
        private float timeRemaining;

        void Start()
        {
            
            QuestionLoad ql = gameObject.GetComponent<QuestionLoad>();
            // Grabs the questions from the DB via the GetQuestions method.
            questionPool = ql.GetQuestions();
            

            selectedColor = Color.green;
            defaultColor = Color.black;

            // Here's where things start happening.  Resets the variables and calls the method to 
            // start showing questions.  Also flips the flag to make the round active and starts
            // the timer.
            playerScore = 0;
            questionIndex = 0;
            numberOfQuestions = questionPool.Count-1;
            ShowQuestion();
            isRoundActive = true;
            UpDateTimeRemainingDisplay();
            selectedAnswers = new List<string>();
        }

        /// <summary>
        /// Cycles through the questions/answers in the dictionary and prints them on the screen.
        /// </summary>
        private void ShowQuestion()
        {
            foreach (var box in answerTexts)
            {
                box.text = "";
                box.color = defaultColor;
                box.gameObject.SetActive(false);
            }
            timeRemaining = RoundData.timeLimitInSeconds;
            // Instantiates the list as we'll use it below.
            correctAnswers = new List<string>();
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
                    correctAnswers.Add(randomList[i].tag);
                }
            }

        }

        /// <summary>
        /// Checks the button clicked to see if it was a right or wrong choice.
        /// </summary>
        /// <param name="button">Passes the button tag</param>
        public void AnswerButtonClicked(GameObject button)
        {
            // Instantiates the TextMeshGUI
            TextMeshProUGUI clickedButton = button.GetComponent<TextMeshProUGUI>();
            
            // Checks the button color.  If it's default, it changes it to show selected and adds the tag to
            // our selectedAnswer list.  If it's already selected, simply changes the color back to default and
            // removes the answer from the selectedAnswer list.
            if (clickedButton.color == selectedColor)
            {
                clickedButton.color = defaultColor;
                selectedAnswers.Remove(button.tag);
            }
            else
            {
                clickedButton.color = selectedColor;
                selectedAnswers.Add(button.tag);
            }
            
        }

        /// <summary>
        /// Checks the selected answers against the correct answers for this question.
        /// </summary>
        public void AnswerCheck()
        {
            
            
            // Right off the bat, we check to see if more answers were selected than correct answers exist.
            // If so, we know they got this answer wrong.
            if (selectedAnswers.Count <= correctAnswers.Count)
            {
                // Then we do a foreach loop to go through all the selected answers, checking each one
                // against our correctAnswer list.  We also remove each correct answer from our correctAnswers list,
                // so we can make sure all correct answers were chosen, below.
                foreach (var answer in selectedAnswers)
                {
                    if (correctAnswers.Contains(answer))
                    {
                        correctAnswers.Remove(answer);
                        Debug.Log("This answer is correct!");
                        
                    }
                    else
                    {
                        Debug.Log("This answer is not correct!");
                       
                    }

                }

                selectedAnswers.Clear();

                // Here we check to make sure there's no more correct answers.  If correctAnswers list is empty,
                // we know they picked all of the correct answers for this question.  Since we checked earlier to see
                // there were no extra answers selected, we can call this question done and award points.
                if (correctAnswers.Count == 0)
                {
                    playerScore += RoundData.pointsAddedForCorrectAnswer;
                    scoreText.text = "Score: " + playerScore;
                    Debug.Log("You got it right!");
                    questionPool.Remove(questionIndex);
                }
                else
                {
                    Debug.Log("You got it wrong, son!");
                }


            }

            //Checks to see if there are any more questions after this one.If so, it increases
            // the question index and moves to the next question.
            if (numberOfQuestions > questionIndex)
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
            GameObject ScoreText = GameObject.FindGameObjectWithTag("Score Display");
            TextMeshProUGUI ScoreMessage = ScoreText.GetComponent<TextMeshProUGUI>();
            ScoreMessage.text = "You scored " + playerScore / 10 + " out of " + numberOfQuestions + " correct!";
        }


        public void ReviewQuestions()
        {
            questionDisplay.SetActive(true);
            // Shows the game over screen.
            roundEndDisplay.SetActive(false);

            questionIndex = 0;

            foreach (var box in answerTexts)
            {
                box.text = "";
                box.color = defaultColor;
                box.gameObject.SetActive(false);
            }
            
            // Prints the current question.
            questionText.text = questionPool[questionIndex].Question;
            

            // Simple for loop to print all our answers, no matter how many there are.
            for (int i = 0; i < questionPool[questionIndex].AnswerPool.Count; i++)
            {
                // Sets the gameobject to active so it can be printed to.  I've turned them off because if there
                // are only two answers to be printed, we don't want the user to be able to click on invisible buttons 3 and 4.
                answerTexts[i].gameObject.SetActive(true);
                Button answerButton = answerTexts[i].gameObject.GetComponent<Button>();
                answerButton.gameObject.SetActive(false);

                // Prints the answer to our scrambled text box.
                answerTexts[i].text = questionPool[questionIndex].AnswerPool[i];
                // Checks to see if the answer currently being printed is a correct one.
                if (questionPool[questionIndex].isCorrect[i])
                {
                    // If so, adds the button tag to our correct answer list.
                    answerTexts[i].color = selectedColor;
                }
            }
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
                    AnswerCheck();
                }


            }

        }

    }
}
