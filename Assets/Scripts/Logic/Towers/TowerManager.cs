using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Logic.Towers
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
            _inputShell.Input.BuildMode.Quit.performed += _ => ChooseNone();
            TowersSoles = new Dictionary<Tower, SolePlatform>();
        }

        public void ChooseTowerOnBtn() //метод реагирующий на выбор башни в меню
        {
            //узнаем какая башня была выбрана из скрипта, находяшегося в кнопке
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<BtnTowerInfo>();
            //прерываем функцию, сели была выбрана не кнопка
            if (button == null)
            {
                Debug.LogError($"Current chosen object is not a button, but it is: {EventSystem.current.currentSelectedGameObject}");
                return;
            }
            ChooseTower(button.TowerType);
            //настравиваем реакцию на клавишы в input-e
            _inputShell.SetBuildingMode();
        }

        public void ChooseTower(Tower type)
        {
            if (type != null)
            {
                _chosenTower = Instantiate(type);
                _chosenTower.Init(false);
            }
            else
            {
                //удаляем выбранную юашню (она не видима для пользователя, но находится на сцене)
                if (!(_chosenTower is null))
                {
                    Destroy(_chosenTower.gameObject);
                }
                _chosenTower = null;
            }
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
