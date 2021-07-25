﻿using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIBehaviour : MonoBehaviour
    {
        [SerializeField] private Text infoText = default;
        [SerializeField] private AudioManager audioManager = default;
        [SerializeField] private Slider audioSlider = default;
        [SerializeField] private Slider musicSlider = default;
        [SerializeField] private LevelMenuBehaviour levelMenu  = default;
        [SerializeField] private Text energyText  = default;
        [SerializeField] private Button skipButton = default;

        private LevelManager _levelManager = default;
        private int _chosenLevelId = 0;
        private float _audioVolume = 1;
        private float _musicVolume = 1;

        public float AudioVolume => _audioVolume;
        public float MusicVolume => _musicVolume;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            if (PlayerPrefs.HasKey(SaveSystem.SaveSystem.AUDIO_SETTINGS_KEY))
            {
                _audioVolume = SaveSystem.SaveSystem.LoadAudio();
            }
            if (PlayerPrefs.HasKey(SaveSystem.SaveSystem.MUSIC_SETTINGS_KEY))
            {
                _musicVolume = SaveSystem.SaveSystem.LoadMusic();
            }
            
            audioManager.UpdateVolume(_audioVolume, _musicVolume);
            if (audioSlider != null) audioSlider.value = _audioVolume;
            if (musicSlider != null) musicSlider.value = _musicVolume;
        }

        private void StartLevel(int levelId)
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
            GoToMainMenu();
        }

        public void ChooseLevel(int newId)
        {
            _chosenLevelId = newId;
            string txt = "";
            switch (newId)
            {
                case 0: {
                    txt = "Day 1. Welcome to Black Tower inc. You will protect our great company from hacker attacks. " +
                          "Hackers will use viruses to steal our data. You must destroy these viruses by using antivirus programs ('towers'). " +
                          "Check out the manual for more information. Beware: sell/upgrade options aren't ready yet and you can only use one antivirus program. " +
                          "Good luck.";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "3 energy/sec";
                } 
                    break;
                case 1:
                {
                    txt = "Day 2. Bad news: today's task will be harder. You will protect 2 paths at once and hackers will use a new powerful virus. " +
                          "However, there are also good news: I've added an ability to sell towers. All the energy invested in the tower returns after selling it. " +
                          "So you can do this as often as you want to.";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "1 energy/sec";
                } 
                    break;
                case 2:
                {
                    txt = "Day 3. You got along with the selling option quite well. So, I added a new ability- now you can upgrade towers. " +
                          "Also, I've added Field Tower. It will slow down the viruses. You're welcome.";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "1 energy/sec";
                } 
                    break;
                case 3:
                {
                    txt = "Day 4. I've added a new antivirus program called 'Rocket launcher'. But hackers weren't sitting on their hands too. " +
                          "Their new virus is really tough. Check out the manual if you want to know more.";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "2 energy/sec";
                } 
                    break;
                case 4:
                {
                    txt = "Day 5. I’m glad you did it, but there is no time to celebrate - hackers started to use another virus. " +
                          "This one is extremely fast, you should be careful!";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "1 energy/sec";
                } 
                    break;
                case 5:
                {
                    txt = "Day 6. I have some bad news: this part of our system is highly vulnerable. " +
                          "However, there are also good news: now you have enough energy to build a strong defense. " +
                          "I hope it will be strong enough to stop the hackers.";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "2 energy/sec";
                } 
                    break;
                case 6:
                {
                    txt = "Day 7. We’ve identified the hackers! Our security units are already on the way, " +
                          "but the hackers are still trying to steal our data. Looks like they mean serious business. Be ready, this is our final round!";
                    skipButton.gameObject.SetActive(true);
                    energyText.text = "2 energy/sec";
                }  
                    break;
                case 7:
                {
                    txt = "???";
                    skipButton.gameObject.SetActive(false);
                    energyText.text = "0 energy/sec";
                }  
                    break;
            }
            infoText.text = txt;
        }
        public void StartChosenLevel()
        {
            StartLevel(_chosenLevelId);
        }
        
        public void SkipChosenLevel()
        {
            SaveSystem.SaveSystem.CompleteLevel(_chosenLevelId);
            levelMenu.OpenLevel(_chosenLevelId + 1);
            ChooseLevel(_chosenLevelId + 1);
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void OnAudioSliderChange(float volume)
        {
            _audioVolume = volume;
            audioManager.UpdateVolume(_audioVolume, _musicVolume);
        }

        public void OnMusicSliderChange(float volume)
        {
            _musicVolume = volume;
            audioManager.UpdateVolume(_audioVolume, _musicVolume);
        }

        private void OnDestroy()
        {
            SaveSystem.SaveSystem.SaveAudio(_audioVolume);
            SaveSystem.SaveSystem.SaveMusic(_musicVolume);
        }

        public void OpenScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
