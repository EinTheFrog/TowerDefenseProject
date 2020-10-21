using UnityEngine;

namespace UI
{
    public class BtnTowerInfo : MonoBehaviour
    {
        [SerializeField]
        Tower towerType = null;
        public Tower TowerType => towerType;
    }
}
