using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {
        [SerializeField] private Text infoText = default;
        
        private LevelManager _levelManager = default;
        private int _chosenLevelId = 0;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        public void StartLevel(int levelId)
        {
            Time.timeScale = 1f;
            var levelName = "Level " + levelId;
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        public void StartNext()
        {
            var nextLevelId = _levelManager.LevelId + 1;
            Time.timeScale = 1f;
            var levelName = "Level " + nextLevelId;
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        }

        public void ChooseLevel(int newId)
        {
            _chosenLevelId = newId;
            string txt = "";
            switch (newId)
            {
                case 0: txt = @"Welcome to the Horns and hooves industries. You will protect our great company from
hacker attacks. They will try to steal our data using different viruses and you should
destroy these viruses until we will define geolocation of these hackers and... 
persuade them to stop attacking us."; 
                    break;
                case 1: txt = "Day 1"; 
                    break;
                case 2: txt = "Day 2"; 
                    break;
                case 3: txt = "Day 3"; 
                    break;
                case 4: txt = "Day 4"; 
                    break;
                case 5: txt = "Day 5"; 
                    break;
                case 6: txt = "Day 6"; 
                    break;
                case 7: txt = "Day 7"; 
                    break;
                case 8: txt = "Day 8"; 
                    break;
                case 9: txt = "Day 9"; 
                    break;
            }
            infoText.text = txt;
        }
        public void StartChosenLevel()
        {
            StartLevel(_chosenLevelId);
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
