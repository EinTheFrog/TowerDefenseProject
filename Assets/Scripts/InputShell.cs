using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputShell : MonoBehaviour
{
    private static readonly Input input = new Input();
    public static Input Instance
    {
        get
        {
            return input;
        }
    }
}
