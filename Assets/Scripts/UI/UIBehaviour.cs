using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {

        private LevelManager _levelManager;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        public void StartGame( int levelId )
        {
            Time.timeScale = 1f;
            var levelName = "Level " + levelId;
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_levelManager.LevelId, LoadSceneMode.Single);
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

        public void ClearMemory()
        {
            SaveSystem.SaveSystem.ClearMemory();
        }
    }
}
