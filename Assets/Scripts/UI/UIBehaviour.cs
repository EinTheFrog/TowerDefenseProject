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
                case 0: txt = @"Welcome to the Horns and hooves industries. You will protect our great company from
hacker attacks. They will try to steal our data using different viruses and you should
destroy these viruses until we will define geolocation of these hackers and... 
persuade them to stop attacking us."; 
                    break;
                case 1: txt = "Day 2"; 
                    break;
                case 2: txt = "Day 3"; 
                    break;
                case 3: txt = "Day 4"; 
                    break;
                case 4: txt = "Day 5"; 
                    break;
                case 5: txt = "Day 6"; 
                    break;
                case 6: txt = "Day 7"; 
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
    }
}
