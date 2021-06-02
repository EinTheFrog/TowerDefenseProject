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
            catch (DirectoryNotFoundException e)
            {
                return new bool[0];
            }
            
            bool[] boolArr = new bool[length];
            using (var reader = new BinaryReader(fileStream))
            {
                for (var i = 0; i < length; i++)
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
    }
}
