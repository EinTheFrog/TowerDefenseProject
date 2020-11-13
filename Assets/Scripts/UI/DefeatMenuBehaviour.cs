using UnityEngine;

namespace UI
{
    public class DefeatMenuBehaviour : MonoBehaviour
    {
        private InputShell _inputShell;
        private CanvasGroup _canvasGroup;

        public static DefeatMenuBehaviour Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
        }
        public void Show()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _inputShell.SetDefeatMode();
            Time.timeScale = 0f;
        }
    }
}
