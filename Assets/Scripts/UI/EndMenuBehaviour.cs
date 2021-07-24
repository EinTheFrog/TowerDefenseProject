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
        private Button _restartButton;
        private const string WinButtonsTag = "WinButton";
        private const string LoseButtonsTag = "LoseButton";

        public static EndMenuBehaviour Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _mainMenuBtnText = GameObject.FindGameObjectWithTag(WinButtonsTag).GetComponentInChildren<Text>();
            _restartButton = GameObject.FindGameObjectWithTag(LoseButtonsTag).GetComponent<Button>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        public void Show(bool playerWon)
        {
            if (_canvasGroup == null) return;
            
            Time.timeScale = 0f;

            text.text = playerWon ? "Mission completed" : "Mission failed";
            _mainMenuBtnText.text = playerWon ? "Complete the day" : "To main menu";
            _restartButton.gameObject.SetActive(!playerWon);
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _inputShell.SetDefeatMode();
        }
    }
}
