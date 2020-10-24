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
            _inputShell.Input.MovementMode.Enable();
            _inputShell.Input.ViewMode.Enable();
            _inputShell.Input.ViewMode.CallMenu.performed += _ => CallMenu();
            _inputShell.Input.MenuMode.CloseMenu.performed += _ => CloseMenu();

            CloseMenu();
        }

        private void CallMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _inputShell.Input.MovementMode.Disable();
            _inputShell.Input.ViewMode.Disable();
            _inputShell.Input.MenuMode.Enable();
            Time.timeScale = 0f;
        }

        public void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _inputShell.Input.MenuMode.Disable();
            _inputShell.Input.ViewMode.Enable();
            _inputShell.Input.MovementMode.Enable();
            Time.timeScale = 1f;
        }
    }
}
