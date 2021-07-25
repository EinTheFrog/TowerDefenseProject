using Gameplay.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerInfoBehaviour : MonoBehaviour
    {
        [SerializeField] private string nameTxtName = default;
        [SerializeField] private string costTxtName = default;
        [SerializeField] private string damageTxtName = default;
        [SerializeField] private string rangeTxtName = default;
        [SerializeField] private string upgradeCostTxtName = default;
        [SerializeField] private string upgradeTxtName = default;
        [SerializeField] private string maxLevelTxtName = default;

        private CanvasGroup _canvasGroup = default;
        private Tower _chosenTower = default;
        private InputShell _inputShell = default;
        private Text _nameText = default;
        private Text _costText = default;
        private Text _upgradeText = default;
        private Text _rangeText = default;
        private Text _damageText = default;
        private Text _upgradeCostText = default;
        private Text _maxLevelText = default;

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
            _damageText = GameObject.Find(damageTxtName).GetComponentInChildren<Text>();
            _rangeText = GameObject.Find(rangeTxtName).GetComponentInChildren<Text>();
            _upgradeCostText = GameObject.Find(upgradeCostTxtName)?.GetComponentInChildren<Text>();
            _upgradeText = GameObject.Find(upgradeTxtName)?.GetComponentInChildren<Text>();
            _maxLevelText = GameObject.Find(maxLevelTxtName)?.GetComponentInChildren<Text>();
        }
        public void CallMenu(Tower chosenTower)
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            _chosenTower = chosenTower;

            _nameText.text = _chosenTower.TowerName;
            _costText.text = "Cost " + _chosenTower.Cost;
            _damageText.text = "Damage " + _chosenTower.Damage;
            _rangeText.text = "Range " + _chosenTower.Range;
            if (_upgradeCostText == null) return;
            _upgradeCostText.text = "Upgrade cost " + _chosenTower.UpgradeCost;
            if (_chosenTower.TowerName == "Field tower")
            {
                _upgradeText.text = "Upgrade gives +slowdown";
            }
            else
            {
                _upgradeText.text = "Upgrade gives +" + _chosenTower.DamagePerLevel + " dmg";
            }
            _maxLevelText.text = "Max level " + _chosenTower.MaxLevel;
        }

        public void UpdateDamage(Tower chosenTower)
        {
            _damageText.text = "Damage " + chosenTower.Damage;
        }

        public void CloseMenu()
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
