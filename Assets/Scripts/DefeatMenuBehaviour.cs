using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatMenuBehaviour : MonoBehaviour
{
    Input input;
    CanvasGroup canvasGroup;

    public static DefeatMenuBehaviour Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        input = InputShell.Instance;
    }
    public void Show()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        input.MovementMode.Disable();
        input.ViewerMode.Disable();
        input.MenuMode.Disable();
        Time.timeScale = 0f;
    }
}
