using UnityEngine;

namespace UI
{
    public class MenuBehaviour : MonoBehaviour
    {
        private InputShell _inputShell;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.ViewMode, CallMenu);
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.MenuMode, CloseMenu);

            CloseMenu();
        }

        private void CallMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _inputShell.SetMenuMode();
            Time.timeScale = 0f;
        }

        public void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _inputShell.SetViewMode();
            Time.timeScale = 1f;
        }
    }
}
