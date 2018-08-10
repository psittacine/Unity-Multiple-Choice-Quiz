using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.QuizGame
{
    public class DataController : MonoBehaviour
    {
 
        void Start () { 
		    // Keeps it ready once the current game ends.
            DontDestroyOnLoad(gameObject);
            // Brings up the menu screen.  For those thinking this is a bit redundant, you're right.
            // My plan is to have a number of different game choices here, such as a quiz game, something a bit more
            // fun, and maybe even some scoreboards. This also provides a handy spot for persistent data, as it's 
            // do not destroy.  Leaving it for now.
            SceneManager.LoadScene ("MenuScreen");
        }

       

        
    
    }
}
