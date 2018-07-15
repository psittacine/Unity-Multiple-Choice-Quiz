using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.QuizGame
{
    public class AnswerButton : MonoBehaviour
    {

        private AnswerData answerData;
        private GameController gmController;
        private string buttonTag;

        // Use this for initialization
        public void Start()
        {

            
            

        }

        // Update is called once per frame
        void Update () {
		
        }

        public void ButtonClicked()
        {
            gmController = FindObjectOfType<GameController>();
            buttonTag = gameObject.tag;
            gmController.AnswerButtonClicked(buttonTag);
        }
        
    }
}
