using System;
using System.IO;
using UI;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystem
    {
        private const string SAVE_FILE_NAME = "saveFile";
        public const string AUDIO_SETTINGS_KEY = "audioVolume";
        public const string MUSIC_SETTINGS_KEY = "musicVolume";

        private static int _mainMenuLevelId = 0;
        public static int MainMenuLevelId => _mainMenuLevelId;

        private static void SaveBoolArray( bool[] boolArr )
        {
            var savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            FileStream fileStream = File.Open(savePath, FileMode.Create);
            using (var writer = new BinaryWriter(fileStream))
            {
                foreach (var b in boolArr)
                {
                    writer.Write(b);
                }
            }
        }

        public static void SaveLevelsStates( bool[] levelsStates )
        {
            SaveBoolArray(levelsStates);
        }
        
        private static bool[] LoadBoolArray( int length )
        {
            var savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            FileStream fileStream;
            try
            {
                fileStream = File.Open(savePath, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
            
            bool[] boolArr = new bool[length];
            using (var reader = new BinaryReader(fileStream))
            {
                for (int i = 0; i < length; i++)
                {
                    boolArr[i] = reader.ReadBoolean();
                }
            }

            return boolArr;
        }

        public static bool[] LoadLevelsStates( int length )
        {
            return LoadBoolArray(length);
        }

        public static void CompleteLevel( int k )
        {
            bool[] levelsStates = LoadLevelsStates(LevelMenuBehaviour.LEVELS_AMOUNT);
            if (levelsStates == null)
            {
                Debug.Log("ATTENTION: Save file hasn't been found");
                return;
            }
            if (k + 1 >= LevelMenuBehaviour.LEVELS_AMOUNT) return;
            levelsStates[k + 1] = true;
            _mainMenuLevelId = k + 1;
            SaveLevelsStates(levelsStates);
        }

        public static void ClearMemory()
        {
            foreach (var directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                DirectoryInfo data_dir = new DirectoryInfo(directory);
                data_dir.Delete(true);
            }
     
            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                FileInfo file_info = new FileInfo(file);
                file_info.Delete();
            }
        }

        public static void SaveAudio(float audioVolume)
        {
            PlayerPrefs.SetFloat(AUDIO_SETTINGS_KEY, audioVolume);
        }

        public static float LoadAudio()
        {
            return PlayerPrefs.GetFloat(AUDIO_SETTINGS_KEY);
        }
        
        public static void SaveMusic(float musicVolume)
        {
            PlayerPrefs.SetFloat(MUSIC_SETTINGS_KEY, musicVolume);
        }

        public static float LoadMusic()
        {
            return PlayerPrefs.GetFloat(MUSIC_SETTINGS_KEY);
        }
    }
}
