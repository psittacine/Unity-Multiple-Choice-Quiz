using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;
using ProBuilder2.Common;
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

        public GameObject FadeFromBlack;
        public Slider TimerSlider;
        public TextMeshProUGUI TimerSeconds;
        public UnityEngine.UI.Button SubmitButton;
        private List<string> categories;
        private Color selectedColor = Color.green;
        private Color defaultColor = Color.white;
        
        

        public void Start()
        {
            // Instantiating our list
            categories = new List<string>();
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
            if (clickedButton.color == selectedColor)
            {
                clickedButton.color = defaultColor;
                categories.Remove(button.name);
            }
            else
            {
                clickedButton.color = selectedColor;
                categories.Add(button.name);
            }
        }

        public void SetTimer()
        {
            TimerSeconds.text = TimerSlider.value.ToString();
            RoundData.timeLimitInSeconds = (int)TimerSlider.value;
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
            foreach (var category in categories)
            {
                queryParameters.Append(category);
                queryParameters.Append(",");
            }

            // Removes that last comma so we don't get an exception.
            queryParameters.Remove(queryParameters.Length-1, 1);
            
            // Assigns the value to the variable in the RoundData class so we can use it in our next scene.
            RoundData.categoryParameters = queryParameters.ToString();
            // Speaking of which.. Loading that new scene.
            SceneManager.LoadScene("QuestionGame");
            // Fading to black
            FadeFromBlack.SetActive(false);
            
        }

        public void Update()
        {
            if (categories.Count == 0)
            {
                SubmitButton.enabled = false;
            }
            else
            {
                SubmitButton.enabled = true;
            }
        }
    }
}
