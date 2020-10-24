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
            var objects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var o in objects)
            {
                Destroy(o);
            }
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
