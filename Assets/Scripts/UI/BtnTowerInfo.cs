using Logic;
using Logic.Towers;
using UnityEngine;

namespace UI
{
    public class BtnTowerInfo : MonoBehaviour
    {
        [SerializeField] private Tower towerType = null;
        public Tower TowerType => towerType;
    }
}
