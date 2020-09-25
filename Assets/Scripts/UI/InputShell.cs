using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputShell : MonoBehaviour
{
    private static readonly Input input = new Input();
    public static Input Instance { get; } = input;

    public static void SetBuildingMode()
    {
        input.BuildingMode.Enable();
        input.ViewMode.Disable();
        CameraMovement.Instance.TriggerOnTowers(false);
    }

    public static void SetViewMode()
    {
        input.ViewMode.Enable();
        input.BuildingMode.Disable();
        CameraMovement.Instance.TriggerOnTowers(true);
    }
}
