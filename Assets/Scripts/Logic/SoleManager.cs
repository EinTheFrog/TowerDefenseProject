using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoleManager : MonoBehaviour
{
    [SerializeField] TowerManager towerManager = null;

    private void OnEnable()
    {
        foreach (SolePlatform sole in GetComponentsInChildren<SolePlatform>()) 
            sole.TowerManager = towerManager;
    }
}
