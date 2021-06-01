using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuBehaviour : MonoBehaviour
{
    private bool[] levelsStates = new bool[10];

    private void Start()
    {
        levelsStates = new Boolean[10] {true, false, false, false, false, false, false, false, false, false};
        var buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (levelsStates[i] == false)
            {
                buttons[i].interactable = false;
            }
        }
    }
}
