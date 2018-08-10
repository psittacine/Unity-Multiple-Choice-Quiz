using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.QuizGame
{
    public class AnswerButton : MonoBehaviour
    {

        private AnswerData answerData;
        private GameController gmController;
        private GameObject button;
        
        /// <summary>
        /// Just a simple buttonclick method.  I could probably combine this, but Unity seems to choke
        /// everytime I try to bring it over to the GameController class.  Still researching why.
        /// </summary>
        public void ButtonClicked()
        {
            gmController = FindObjectOfType<GameController>();
            gmController.AnswerButtonClicked(gameObject);
        }
        
    }
}
