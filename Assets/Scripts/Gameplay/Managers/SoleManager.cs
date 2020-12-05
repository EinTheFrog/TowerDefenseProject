using Gameplay.Platforms;
using UnityEngine;

namespace Gameplay.Managers
{
    public class SoleManager : MonoBehaviour
    {
        [SerializeField] private TowerManager towerManager = null;

        private void OnEnable()
        {
            foreach (var sole in GetComponentsInChildren<SolePlatform>())
            {
                sole.TowerManager = towerManager;
            }
        }
    }
}
