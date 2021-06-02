using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuBehaviour : MonoBehaviour
{
    private const int LEVELS_AMOUNT = 10;
    private bool[] _levelsStates = new bool[LEVELS_AMOUNT];

    private void Start()
    {
        _levelsStates = SaveSystem.SaveSystem.LoadLevelsStates(LEVELS_AMOUNT);
        var buttons = GetComponentsInChildren<Button>();
        for (var i = 0; i < buttons.Length; i++)
        {
            if (_levelsStates[i] == false)
            {
                buttons[i].interactable = false;
            }
        }
    }
}
