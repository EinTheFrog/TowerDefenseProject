using UnityEngine;

namespace Logic
{
    public class SoleManager : MonoBehaviour
    {
        [SerializeField] TowerManager towerManager = null;

        private void OnEnable()
        {
            foreach (SolePlatform sole in GetComponentsInChildren<SolePlatform>())
            {
                sole.TowerManager = towerManager;
            }
        }
    }
}
