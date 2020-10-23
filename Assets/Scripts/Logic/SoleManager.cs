using UnityEngine;

namespace Logic
{
    public class SoleManager : MonoBehaviour
    {
        [SerializeField] TowerManager towerManager = null;

        private void OnEnable()
        {
            foreach (var sole in GetComponentsInChildren<SolePlatform>())
            {
                sole.TowerManager = towerManager;
            }
        }
    }
}
