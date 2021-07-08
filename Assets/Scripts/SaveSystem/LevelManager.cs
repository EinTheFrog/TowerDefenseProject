using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    public class LevelManager : MonoBehaviour
    {
        public int LevelId { get; private set; }

        private void OnEnable()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var number = FindNumberInName(sceneName);
            LevelId = number;
        }

        private static int FindNumberInName(string s)
        {
            string ans = "";
            int i = 0;
            while (i < s.Length)
            {
                if (!Char.IsDigit(s[i]))
                {
                    i++;
                }
                else
                {
                    while (i < s.Length)
                    {
                        ans += s[i];
                        i++;
                        if (i == s.Length || !Char.IsDigit(s[i])) return Int32.Parse(ans);
                    } 
                }
            }

            throw new ArgumentException();
        }
    }
}
