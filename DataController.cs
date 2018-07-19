﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.QuizGame
{
    public class DataController : MonoBehaviour
    {
        
        // Use this for initialization
        void Start () {
		    // Keeps it ready once the current game ends.
            DontDestroyOnLoad(gameObject);
            // Brings up the menu screen.  For those thinking this is a bit redundant, you're right.
            // My plan is to have a number of different choices here, such as category, number of questions,
            // time limits, etc.  Until then, we have an extra screen.
            SceneManager.LoadScene ("MenuScreen");
        }

       

        
    
    }
}
