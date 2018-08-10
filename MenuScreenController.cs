using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.Experimental.UIElements.Button;

namespace Assets.Scripts.QuizGame
{
    public class MenuScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject FadeFromBlack;
        [SerializeField] private Slider TimerSlider;
        [SerializeField] private TextMeshProUGUI TimerSeconds;
        [SerializeField] private TextMeshProUGUI QuestionCount;
        [SerializeField] private UnityEngine.UI.Button SubmitButton;
        [SerializeField] private Toggle RandomizeToggle;
        [SerializeField] private GameObject SettingsMenu;

        private List<int> _numberOfQuestions;
        private int _currentCount;
        private List<string> _categories;
        private Color _selectedColor = Color.green;
        private Color _defaultColor = Color.white;
        
        public void Start()
        {
            // Instantiating our lists
            _categories = new List<string>();
            _numberOfQuestions = new List<int>();
            MenuScreenDAL msd = gameObject.GetComponent<MenuScreenDAL>();
            _numberOfQuestions = msd.GetNumberOfQuestions();
            RoundData.TimeLimitInSeconds = 30;
            RoundData.RandomQuestions = true;
            // Fading from black
            FadeFromBlack.SetActive(false);
        }

        /// <summary>
        /// Tracks which categories are being selected.
        /// </summary>
        /// <param name="button">UnityEngine.UI.Button. Not to be confused for two freaking hours with a normal button....</param>
        public void SelectCategory(UnityEngine.UI.Button button)
        {
            // Image of the UnityEngine.UI.Button (so we can change the color)
            Image clickedButton = button.GetComponent<Image>();
             
            // Same deal as in our GameController script.  Just checks the color to see if it's selected or default
            // and then adds/removes from our list based on what needs to happen.  
            if (clickedButton.color == _selectedColor)
            {
                clickedButton.color = _defaultColor;
                _categories.Remove(button.name);
                _currentCount -= _numberOfQuestions[(Int32.Parse(button.name)) - 1];
                QuestionCount.text = _currentCount.ToString();
            }
            else
            {
                clickedButton.color = _selectedColor;
                _categories.Add(button.name);
                _currentCount += _numberOfQuestions[(Int32.Parse(button.name)) - 1];
                QuestionCount.text = _currentCount.ToString();
            }
        }

        /// <summary>
        /// Sets the timer for each question.
        /// </summary>
        public void SetTimer()
        {
            TimerSeconds.text = TimerSlider.value.ToString();
            RoundData.TimeLimitInSeconds = (int)TimerSlider.value;
        }
       
        /// <summary>
        /// Starts the actual quiz game after the categories have been selected and the Start Game button has been pushed.
        /// </summary>
        public void StartGame()
        {
            // I like Stringbuilders, but never get to use them enough.  Today's the day!
            StringBuilder queryParameters = new StringBuilder();

            // Runs through each of the indexes in our category list, adds them to the stringbuilder, and puts a comma
            // after it for the SQLLite query.
            foreach (var category in _categories)
            {
                queryParameters.Append(category);
                queryParameters.Append(",");
            }

            // Removes that last comma so we don't get an exception.
            queryParameters.Remove(queryParameters.Length-1, 1);
            
            // Assigns the value to the variable in the RoundData class so we can use it in our next scene.
            RoundData.CategoryParameters = queryParameters.ToString();
            // Speaking of which.. Loading that new scene.
            SceneManager.LoadScene("QuestionGame");
        }

        /// <summary>
        /// Unity-specific method, checks it each frame.  Right now we're using it to check the status of the 
        /// start quiz button. If no categories have been selected, it prevents the button from being used.
        /// </summary>
        public void Update()
        {
            SubmitButton.enabled = _categories.Count != 0;
        }

        /// <summary>
        /// Turns the settings menu on and off.
        /// </summary>
        public void MenuToggle()
        {
            SettingsMenu.SetActive(!SettingsMenu.activeSelf);
        }

        /// <summary>
        /// Turns the randomizing of the quiz questions on and off.
        /// </summary>
        public void RandomToggle()
        {
            RoundData.RandomQuestions = !RoundData.RandomQuestions;
        }
    }
}
