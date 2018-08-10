using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.QuizGame
{
    public class GameController : MonoBehaviour
    {


        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI[] _answerTexts;
        [SerializeField] private TextMeshProUGUI _questionText;
        [SerializeField] private TextMeshProUGUI _timeDisplay;
        [SerializeField] private TextMeshProUGUI _questionCounter;
        [SerializeField] private GameObject _escapeMenu;
        [SerializeField] private GameObject _questionDisplay;
        [SerializeField] private GameObject _roundEndDisplay;
        [SerializeField] private GameObject _submitButton;
        [SerializeField] private GameObject _reviewButton;


        private Dictionary<int, AnswerData> _questionPool;
        private Dictionary<int, AnswerData> _reviewPool;
        private Dictionary<int, AnswerData> _randomPool;
        private List<string> _correctAnswers;
        private List<string> _selectedAnswers;
        private int _questionIndex;
        private int _numberOfQuestions;
        private int _playerScore;
        private Color _selectedColor;
        private Color _defaultColor;
        private bool _isRoundActive;
        private float _timeRemaining;
        private bool _pauseTime = false;


        void Start()
        {
            // Resets the buttons just in case we're returning from the end of round screen.
            _submitButton.SetActive(true);
            _reviewButton.SetActive(false);

            // Instantiates the QuestionLoad class.
            QuestionLoad ql = gameObject.GetComponent<QuestionLoad>();

            // Grabs the questions from the DB via the GetQuestions method.
            _questionPool = ql.GetQuestions();
            
            // Sets our colors.  Unity freaks out if this isn't done at Start.  No idea why.
            _selectedColor = Color.green;
            _defaultColor = Color.black;

            // Here's where things start happening.  Resets the variables and calls the method to 
            // start showing questions.  Also flips the flag to make the round active and starts
            // the timer.
            _playerScore = 0;
            _questionIndex = 0;
            _numberOfQuestions = _questionPool.Count-1;
            _isRoundActive = true;
            UpDateTimeRemainingDisplay();
            _selectedAnswers = new List<string>();
            _reviewPool = new Dictionary<int, AnswerData>();
            _randomPool = new Dictionary<int, AnswerData>();
            
            // Checks to see if the questions need to be randomized.
            if (RoundData.RandomQuestions)
            {
                List<int> newIndex = new List<int>();
                for (int i = 0; i < _questionPool.Count; i++)
                {
                    newIndex.Add(i);
                }
                // Truly randomizing a dictionary isn't straightforward. Hence, the list setting a new assortment of keys.
                var rand = new System.Random();
                newIndex = newIndex.OrderBy(x => rand.Next()).ToList();
                for (int i = 0; i < _questionPool.Count; i++)
                {
                    _randomPool[newIndex[i]] = _questionPool[i];
                }
                
                // Replacing our ordered questions with the new, scrambled ones.
                _questionPool = _randomPool;
            }

            ShowQuestion();
        }

        /// <summary>
        /// Cycles through the questions/answers in the dictionary and prints them on the screen.
        /// </summary>
        private void ShowQuestion()
        {
            _questionCounter.text = "Question: " + (_questionIndex + 1) + " of " + _questionPool.Count;

            ResetAnswerBoxes();
            
            _timeRemaining = RoundData.TimeLimitInSeconds;

            // Instantiates the list as we'll use it below.
            _correctAnswers = new List<string>();

            // Prints the current question.
            _questionText.text = _questionPool[_questionIndex].Question;

            // We're going to mix up our answers here.  Easiest way I found to do this (and still keep
            // track of whether the answer is correct without creating another class) was to mix up 
            // the text boxes instead of the answers themselves.  That way the isCorrect and answers still match up.
            // Likely a better way of doing it, but...  ¯\_(ツ)_/¯
            var rand = new System.Random();

            // Scrambles the order of the text boxes and assigns them to the new list.
            var randomList = _answerTexts.OrderBy(x => rand.Next()).ToList();

            // Simple for loop to print all our answers, no matter how many there are.
            for (int i = 0; i < _questionPool[_questionIndex].AnswerPool.Count; i++)
            {
                // Sets the gameobject to active so it can be printed to.  I've turned them off because if there
                // are only two answers to be printed, we don't want the user to be able to click on invisible buttons 3 and 4.
                randomList[i].gameObject.SetActive(true);

                // Prints the answer to our scrambled text box.
                randomList[i].text = _questionPool[_questionIndex].AnswerPool[i];

                // Checks to see if the answer currently being printed is a correct one.
                if (_questionPool[_questionIndex].IsCorrect[i])
                {
                    // If so, adds the button tag to our correct answer list.
                    _correctAnswers.Add(randomList[i].tag);
                }
            }

        }
        
        /// <summary>
        /// Changes the color and either adds or removes the answer clicked from our _selectedAnswers list.
        /// </summary>
        /// <param name="button">Passes the button tag</param>
        public void AnswerButtonClicked(GameObject button)
        {
            // Instantiates the TextMeshGUI
            TextMeshProUGUI clickedButton = button.GetComponent<TextMeshProUGUI>();
            
            // Checks the button color.  If it's default, it changes it to show selected and adds the tag to
            // our selectedAnswer list.  If it's already selected, simply changes the color back to default and
            // removes the answer from the selectedAnswer list.
            if (clickedButton.color == _selectedColor)
            {
                clickedButton.color = _defaultColor;
                _selectedAnswers.Remove(button.tag);
            }
            else
            {
                clickedButton.color = _selectedColor;
                _selectedAnswers.Add(button.tag);
            }
            
        }

        /// <summary>
        /// Checks the selected answers against the correct answers for this question.
        /// </summary>
        public void CheckForCorrectAnswers()
        {
            // Right off the bat, we check to see if more answers were selected than correct answers exist.
            // If so, we know they got this answer wrong.
            if (_selectedAnswers.Count <= _correctAnswers.Count)
            {
                // Then we do a foreach loop to go through all the selected answers, checking each one
                // against our correctAnswer list.  We also remove each correct answer from our correctAnswers list,
                // so we can make sure all correct answers were chosen, below.
                foreach (var answer in _selectedAnswers)
                {
                    if (_correctAnswers.Contains(answer))
                    {
                        _correctAnswers.Remove(answer);
                        
                        // Debug was for my own sanity and serves no other purpose.  Leaving it in so you can error check if need be.
                        Debug.Log("This answer is correct!");
                    }
                    else
                    {
                        Debug.Log("This answer is not correct!");
                        // We add the question to our review pool for later... uh... review
                        _reviewPool.Add(_reviewPool.Count, _questionPool[_questionIndex]);
                    } 

                }
                

                // Here we check to make sure there are no more correct answers.  If correctAnswers list is empty,
                // we know they picked all of the correct answers for this question.  Since we checked earlier to see
                // there were no extra answers selected, we can call this question done and award points.
                if (_correctAnswers.Count == 0)
                {
                    _playerScore += RoundData.PointsAddedForCorrectAnswer;
                    _scoreText.text = "Score: " + _playerScore;
                    Debug.Log("You got it right!");
                    
                }
                else
                {
                    Debug.Log("You got it wrong, son!");
                    _reviewPool.Add(_reviewPool.Count,_questionPool[_questionIndex]);
                }
            }
            else
            {
                // Add it to the review pool.
                _reviewPool.Add(_reviewPool.Count, _questionPool[_questionIndex]);
            }

            // Now we clear the selected answers for the next question. 
            _selectedAnswers.Clear();

            //Checks to see if there are any more questions after this one.If so, it increases
            // the question index and moves to the next question.
            if (_numberOfQuestions > _questionIndex)
            {
                _questionIndex++;
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
            if (_escapeMenu.activeSelf)
            { _escapeMenu.SetActive(false);}
            // Swaps the flag to false
            _isRoundActive = false;
            // Turns off the quiz screen
            _questionDisplay.SetActive(false);
            // Shows the game over screen.
            _roundEndDisplay.SetActive(true);
            GameObject ScoreText = GameObject.FindGameObjectWithTag("Score Display");
            TextMeshProUGUI ScoreMessage = ScoreText.GetComponent<TextMeshProUGUI>();
            ScoreMessage.text = "You scored " + _playerScore / 10 + " out of " + (_numberOfQuestions + 1) + " correct!";
        }

        public void ReviewStart()
        {
            _questionDisplay.SetActive(true);
            _submitButton.SetActive(false);
            _reviewButton.SetActive(true);
            _roundEndDisplay.SetActive(false);
            _questionIndex = 0;
            _numberOfQuestions = _reviewPool.Count - 1;
            ReviewQuestions();

        }

        public void ReviewQuestions()
        {

            _questionCounter.text = "Question: " + (_questionIndex + 1) + " of " + _reviewPool.Count;

            ResetAnswerBoxes();
            
            // Prints the current question.
            _questionText.text = _reviewPool[_questionIndex].Question;
            

            // Simple for loop to print all our answers, no matter how many there are.
            for (int i = 0; i < _reviewPool[_questionIndex].AnswerPool.Count; i++)
            {
                // Sets the gameobject to active so it can be printed to.  I've turned them off because if there
                // are only two answers to be printed, we don't want the user to be able to click on invisible buttons 3 and 4.
                
                _answerTexts[i].gameObject.SetActive(true);
                Button answerButton = _answerTexts[i].gameObject.GetComponent<Button>();
                answerButton.interactable = false;

                
                // Prints the answer to our scrambled text box.
                _answerTexts[i].text = _reviewPool[_questionIndex].AnswerPool[i];
                // Checks to see if the answer currently being printed is a correct one.
                if (_reviewPool[_questionIndex].IsCorrect[i])
                {
                    // If so, adds the button tag to our correct answer list.
                    _answerTexts[i].color = _selectedColor;
                    
                }
            }
        }

        public void NextReviewQuestion()
        {
            if (_numberOfQuestions > _questionIndex)
            {
                _questionIndex++;
                ReviewQuestions();
            }
            else
            {
                // Swaps the flag to false
                _isRoundActive = false;
                
                ReturnToMenu();
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
            _timeDisplay.text = "Time: " + Mathf.Round(_timeRemaining);
        }


        /// <summary>
        /// Resets the answer boxes for the next question.
        /// </summary>
        public void ResetAnswerBoxes()
        {
            foreach (var box in _answerTexts)
            {
                box.text = "";
                box.color = _defaultColor;
                box.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Checks the flag to see if a round is currently underway.
            if ((_isRoundActive) && (!_pauseTime))
            {
                if (RoundData.TimeLimitInSeconds != 0)
                {
                    // Counts down the time from the time remaining variable.
                    _timeRemaining -= Time.deltaTime;
                UpDateTimeRemainingDisplay();

                
                    // Checks to see if time ran out.
                    if (_timeRemaining <= 0f)
                    {
                        CheckForCorrectAnswers();
                    }
                }

            }

            MenuToggle();

        }


        public void MenuToggle()
        {
            if (Input.GetKeyDown("escape") && _escapeMenu.activeSelf == false)
            {
                _pauseTime = true;
                _escapeMenu.SetActive(true);
                _questionDisplay.SetActive(false);
            }
            else if (Input.GetKeyDown("escape") && _escapeMenu.activeSelf == true)
            {
                _pauseTime = false;
                _escapeMenu.SetActive(false);
                _questionDisplay.SetActive(true);
            }
        }

    }
}
