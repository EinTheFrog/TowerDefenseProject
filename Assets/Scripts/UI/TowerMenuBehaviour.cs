using Logic;
using Logic.Towers;
using UnityEngine;

namespace UI
{
    public class TowerMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private BuildingMenuBehaviour buildingMenu = null;
        [SerializeField] private InputShell inputShell = null;
        
        private CanvasGroup _canvasGroup;
        private Tower _chosenTower;
        
        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            //inputShell.Input.TowerMode.Close.performed += _ => CloseMenu();
            inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.TowerMode, CloseMenu);
            CloseMenu();
        }
        public void CallMenu(Tower chosenTower)
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            buildingMenu.CloseMenu();
            inputShell.SetTowerMode();
            
            _chosenTower = chosenTower;
        }

        private void CloseMenu()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            inputShell.SetViewMode();
            buildingMenu.CallMenu();
        }

        public void SellTower()
        {
            _chosenTower.Destroy();
            CloseMenu();
        }
    }
}
