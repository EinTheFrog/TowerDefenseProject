using Gameplay.Managers;
using Gameplay.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private BuildingMenuBehaviour buildingMenu = default;
        [SerializeField] private TowerManager towerManager = default;
        [SerializeField] private Text levelText = default;

        private CanvasGroup _canvasGroup = default;
        private Tower _chosenTower = default;
        private InputShell _inputShell = default;

        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.TowerMode, CloseMenu);
            _inputShell.SetActionForMode(InputShell.ActionType.ShowAdditionalInfo, InputShell.Mode.ViewMode, towerManager.ShowTowersLevels);
            _inputShell.SetActionForMode(InputShell.ActionType.ShowAdditionalInfo, InputShell.Mode.BuildMode, towerManager.ShowTowersLevels);
            _inputShell.SetActionForMode(InputShell.ActionType.ShowAdditionalInfo, InputShell.Mode.TowerMode, towerManager.ShowTowersLevels);
            CloseMenu();
        }
        public void CallMenu(Tower chosenTower)
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            buildingMenu.CloseMenu();
            _inputShell.SetTowerMode();

            if (_chosenTower != null)
            {
                _chosenTower.showChose(false);
            }
            _chosenTower = chosenTower;
            SetLevelText(_chosenTower.Level);
            _chosenTower.showChose(true);
        }

        private void SetLevelText(int level)
        {
            levelText.text = "Level " + level;
        }

        private void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _inputShell.SetViewMode();
            buildingMenu.CallMenu();
            
            if (_chosenTower == null) return;
            _chosenTower.showChose(false);
            _chosenTower = null;
        }
        
        public void SellTower()
        {
            towerManager.SellTower(_chosenTower);
            CloseMenu();
        }
        
        public void UpgradeTower()
        {
            towerManager.UpgradeTower(_chosenTower);
            SetLevelText(_chosenTower.Level);
        }
    }
}
