using Gameplay.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerInfoBehaviour : MonoBehaviour
    {
        [SerializeField] private string nameTxtName;
        [SerializeField] private string costTxtName;
        [SerializeField] private string upgradeTxtName;
        [SerializeField] private string rangeTxtName;
        [SerializeField] private string damageTxtName;
        
        private CanvasGroup _canvasGroup = default;
        private Tower _chosenTower = default;
        private InputShell _inputShell = default;
        private Text _nameText = default;
        private Text _costText = default;
        private Text _upgradeText = default;
        private Text _rangeText = default;
        private Text _damageText = default;

        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.BuildMode, CloseMenu);
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.TowerMode, CloseMenu);
            CloseMenu();

            _nameText = GameObject.Find(nameTxtName).GetComponentInChildren<Text>();
            _costText = GameObject.Find(costTxtName).GetComponentInChildren<Text>();
            _upgradeText = GameObject.Find(upgradeTxtName).GetComponentInChildren<Text>();
            _rangeText = GameObject.Find(rangeTxtName).GetComponentInChildren<Text>();
            _damageText = GameObject.Find(damageTxtName).GetComponentInChildren<Text>();
        }
        public void CallMenu(Tower chosenTower)
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            _chosenTower = chosenTower;

            _nameText.text = _chosenTower.name;
            _costText.text = "Cost " + _chosenTower.Cost + "/Upgrade cost " + _chosenTower.UpgradeCost;
            _upgradeText.text = "+" + _chosenTower.DangerChangePerLevel + "dmg/";
            _rangeText
        }

        private void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _inputShell.SetViewMode();

            _chosenTower = null;
        }
    }
}
