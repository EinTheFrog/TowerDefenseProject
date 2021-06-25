using System;
using UnityEngine;

namespace UI
{
    public class BuildingMenuBehaviour : MonoBehaviour
    {
        private InputShell _inputShell;
        private CanvasGroup _canvasGroup;

        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.ViewMode, CloseMenu);
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.MenuMode, CallMenu);
            CallMenu();
        }

        public void CallMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}
