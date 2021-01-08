using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {
        public void StartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Level", LoadSceneMode.Single);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
