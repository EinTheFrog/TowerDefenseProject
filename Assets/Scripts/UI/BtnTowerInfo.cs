using UnityEngine;

public class BtnTowerInfo : MonoBehaviour
{
    [SerializeField]
    Tower towerType = null;
    public Tower ButtonsTowerType { get { return towerType; } }
}
