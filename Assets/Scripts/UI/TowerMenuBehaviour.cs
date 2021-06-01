using Gameplay.Towers;
using UnityEngine;

namespace UI
{
    public class TowerMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private BuildingMenuBehaviour buildingMenu = null;

        private CanvasGroup _canvasGroup;
        private Tower _chosenTower;
        private InputShell _inputShell = null;
        
        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.TowerMode, CloseMenu);
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
            
            _chosenTower = chosenTower;
        }

        private void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _inputShell.SetViewMode();
            buildingMenu.CallMenu();
        }

        public void SellTower()
        {
            _chosenTower.Sell();
            CloseMenu();
        }
    }
}
