using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Level", LoadSceneMode.Single);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
