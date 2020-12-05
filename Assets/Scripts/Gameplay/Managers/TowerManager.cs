using System.Collections.Generic;
using Gameplay.Enemies;
using Gameplay.Platforms;
using Gameplay.Towers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Managers
{
    public class TowerManager : MonoBehaviour
    {
        [SerializeField] private RoadManager roadManager = null;
        [SerializeField] private MoneyManager moneyManager = null;

        private Tower _chosenTower;
        private InputShell _inputShell;
        
        public Dictionary<Tower, SolePlatform> TowersSoles { get; private set; }

        private void Start()
        {
            //добавляем реакции на нажатие клавиш
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            //_inputShell.Input.BuildMode.Quit.performed += _ => ChooseNone();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.BuildMode, ChooseNone);
            TowersSoles = new Dictionary<Tower, SolePlatform>();
        }

        public void ChooseTowerOnBtn()
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<BtnTowerInfo>();
            if (button == null)
            {
                Debug.LogError($"Current chosen object is not a button, but it is: {EventSystem.current.currentSelectedGameObject}");
                return;
            }
            ChooseTower(button.TowerType);
            _inputShell.SetBuildingMode();
        }

        public void ChooseTower(Tower type)
        {
            if (_chosenTower != null)
            {
                Destroy(_chosenTower.gameObject);
            }
            _chosenTower = null;
            
            if (type == null) return;
            
            _chosenTower = Instantiate(type);
            _chosenTower.Init(false);
        }

        private void ChooseNone() //метод реагирующий на выход из режима постройки
        {
           ChooseTower(null);
           //настравиваем реакцию на клавишы в input-e
           _inputShell.SetViewMode();
        }

        public bool BuyChosenTower(SolePlatform sole)
        {
            if (_chosenTower.Cost > moneyManager.Money) return false;
            moneyManager.Money -= _chosenTower.Cost;
            BuildChosenTower(sole);
            return true;
        }

        public void GetMoneyForKill(Enemy enemy) => moneyManager.Money += enemy.Reward;

        private void BuildChosenTower(SolePlatform sole)
        {
            //создаем копию призрака (выбранного строения) в указанном месте
            var newTower = Instantiate(_chosenTower);
            newTower.Remove += RemoveTower;
            if (TowersSoles == null)
            {
                TowersSoles = new Dictionary<Tower, SolePlatform>();
            }
            TowersSoles[newTower] = sole;
            newTower.Init(Tower.TowerState.Building, sole.Center, this);
            //ищем дороги в радиусе поражения и меняем их опасность
            roadManager.UpdateDangerInRadius(sole.Center, _chosenTower.transform.GetComponentInChildren<EnemyTrigger>().Radius, 1);
        }

        public void AddMoney(int amount) => moneyManager.Money += amount;

        private void RemoveTower(Tower tower)
        {
            roadManager.UpdateDangerInRadius(TowersSoles[tower].Center, tower.transform.GetComponentInChildren<EnemyTrigger>().Radius, -1);

            TowersSoles[tower].IsFree = true;
            TowersSoles.Remove(tower);
        }

        public void ShowChosenTower(SolePlatform sole)
        {
            //показываем призрак строения в зависимости от занятости фундамента
            _chosenTower.Init(sole.IsFree &&  _chosenTower.Cost <= moneyManager.Money ? 
                Tower.TowerState.GreenGhost : Tower.TowerState.RedGhost, sole.Center, this);
        }

        public void HideTower()
        {
            _chosenTower.Init(false);
        }
    }
}
