using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Logic
{
    public class TowerManager : MonoBehaviour
    {
        [SerializeField] private RoadManager roadManager = null;

        private Input _input;
        private Tower _chosenTower;
        public Dictionary<Tower, SolePlatform> TowersSoles;

        public RoadManager RoadManager => roadManager;


        private void OnEnable()
        {
            //узнаем актуальный input (общий для всех)
            _input = InputShell.Instance;
            //добавляем реакции на нажатие клавиш
            _input.BuildingMode.Quit.performed += _ => ChooseNone();
        }

        private void Start()
        {
            TowersSoles = new Dictionary<Tower, SolePlatform>();
        }

        public void ChooseTowerOnBtn() //метод реагирующий на выбор башни в меню
        {
            //узнаем какая башня была выбрана из скрипта, находяшегося в кнопке
            BtnTowerInfo button = EventSystem.current.currentSelectedGameObject.GetComponent<BtnTowerInfo>();
            //прерываем функцию, сели была выбрана не кнопка
            if (button == null)
            {
                Debug.LogError($"Current chosen object is not a button, but it is: {EventSystem.current.currentSelectedGameObject}");
                return;
            }
            ChooseTower(button.TowerType);
            //настравиваем реакцию на клавишы в input-e
            InputShell.SetBuildingMode();
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
                Destroy(_chosenTower.gameObject);
                _chosenTower = null;
            }
        }

        public void ChooseNone() //метод реагирующий на выход из режима постройки
        {
           ChooseTower(null);
           //настравиваем реакцию на клавишы в input-e
           InputShell.SetViewMode();
        }

        public void BuildChosenTower(SolePlatform sole)
        {
            //создаем копию призрака (выбранного строения) в указанном месте
            var newTower = Instantiate(_chosenTower);
            newTower.Remove += RemoveTower;
            if (TowersSoles == null)
            {
                TowersSoles = new Dictionary<Tower, SolePlatform>();
            }
            TowersSoles[newTower] = sole;
            newTower.Init(Tower.TowerState.Building, sole.Center);
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
            if (sole.IsFree) _chosenTower.Init(Tower.TowerState.GreenGhost, sole.Center);
            else _chosenTower.Init(Tower.TowerState.RedGhost, sole.Center);
        }

        public void HideTower()
        {
            _chosenTower.Init(false);
        }
    }
}
