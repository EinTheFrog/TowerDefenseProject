using UnityEngine;

namespace SaveSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private int levelNumber;

        public int LevelNumber => levelNumber;
    }
}
