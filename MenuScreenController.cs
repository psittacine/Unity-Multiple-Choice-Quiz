using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.QuizGame
{
    public class MenuScreenController : MonoBehaviour
    {

        public GameObject FadeFromBlack;
  
        

        public void StartGame()
        {
            SceneManager.LoadScene("QuestionGame");
            FadeFromBlack.SetActive(false);
            
        }
    }
}
