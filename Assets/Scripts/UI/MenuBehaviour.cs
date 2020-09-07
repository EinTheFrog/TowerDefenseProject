using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    Input input;
    CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        input = InputShell.Instance;
        input.MovementMode.Enable();
        input.ViewMode.Enable();
        input.ViewMode.CallMenu.performed += _ => CallMenu();
        input.MenuMode.CloseMenu.performed += _ => CloseMenu();

        CloseMenu();
    }

    private void CallMenu()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        input.MovementMode.Disable();
        input.ViewMode.Disable();
        input.MenuMode.Enable();
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        input.MenuMode.Disable();
        input.ViewMode.Enable();
        input.MovementMode.Enable();
        Time.timeScale = 1f;
    }
}
