using System;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystem
    {
        private const string SAVE_FILE_NAME = "saveFile";

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
            if (k + 1 >= LevelMenuBehaviour.LEVELS_AMOUNT) return;
            levelsStates[k + 1] = true;
            SaveLevelsStates(levelsStates);
        }
    }
}
