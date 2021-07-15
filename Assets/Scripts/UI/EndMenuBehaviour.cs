using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace UI
{
    public class EndMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private Text text = default;
        
        private InputShell _inputShell;
        private CanvasGroup _canvasGroup;
        private Text _mainMenuBtnText;
        private const string _winButtonsTag = "WinButton";

        public static EndMenuBehaviour Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _mainMenuBtnText = GameObject.FindGameObjectWithTag(_winButtonsTag).GetComponentInChildren<Text>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        public void Show(bool playerWon)
        {
            if (_canvasGroup == null) return;
            
            Time.timeScale = 0f;

            text.text = playerWon ? "You won" : "You lose";
            _mainMenuBtnText.text = playerWon ? "Complete level" : "To main menu";
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _inputShell.SetDefeatMode();
        }
    }
}
