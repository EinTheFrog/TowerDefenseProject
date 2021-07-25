using System.Collections.Generic;
using Gameplay.Enemies;
using Gameplay.Platforms;
using Gameplay.Towers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UIBehaviour = UI.UIBehaviour;

namespace Gameplay.Managers
{
    public class TowerManager : MonoBehaviour
    {
        [SerializeField] private RoadManager roadManager = null;
        [SerializeField] private MoneyManager moneyManager = null;
        [SerializeField] private UIBehaviour uiManager = null;

        private Tower _chosenTower;
        private InputShell _inputShell;
        private bool _showingLevels = true;
        private TowerInfoBehaviour _towerInfo;
        public bool ShowingLevels => _showingLevels;

        public Dictionary<Tower, SolePlatform> TowersSoles { get; private set; }
        
        public delegate void LevelShowHandler(bool shouldShow);
        public event LevelShowHandler ShowLevel;
        public delegate void RotationHandler(Vector3 v);
        public event RotationHandler UpdateRotation;

        private void Start()
        {
            //добавляем реакции на нажатие клавиш
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            //_inputShell.Input.BuildMode.Quit.performed += _ => ChooseNone();
            _inputShell.SetActionForMode(InputShell.ActionType.Cancel, InputShell.Mode.BuildMode, ChooseNone);
            TowersSoles = new Dictionary<Tower, SolePlatform>();
            _towerInfo = FindObjectOfType<TowerInfoBehaviour>().GetComponent<TowerInfoBehaviour>();
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
            _towerInfo.CallMenu(_chosenTower);
            _inputShell.SetBuildingMode();
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

        public void AddMoney(int amount) => moneyManager.Money += amount;
        
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
            float towerAudioMultiplier = 1f;
            switch (_chosenTower.TowerName)
            {
                case "Laser beam": towerAudioMultiplier = 0.2f;
                    break;
                case "Field tower": towerAudioMultiplier = 0.15f;
                    break;
                case "Rocket launcher": towerAudioMultiplier = 0.4f;
                    break;
            }
            newTower.Init(Tower.TowerState.Building, sole.Center, this, uiManager.AudioVolume * towerAudioMultiplier);
            //ищем дороги в радиусе поражения и меняем их опасность
            var chosenTowerTransform = _chosenTower.transform;
            var rad = chosenTowerTransform.GetComponentInChildren<EnemyTrigger>().Radius;
            //берем масштаб любого измерения
            var scale = chosenTowerTransform.localScale.x;
            roadManager.UpdateDangerInRadius(sole.Center, rad * scale, newTower.BasicDanger);
        }
        
        public void UpgradeTower(Tower chosenTower)
        {
            if (chosenTower.UpgradeCost > moneyManager.Money || chosenTower.Level == chosenTower.MaxLevel) return;
            chosenTower.Upgrade();
            moneyManager.Money -= chosenTower.UpgradeCost;
            //ищем дороги в радиусе поражения и меняем их опасность
            var towerTransform = chosenTower.transform;
            var rad = towerTransform.GetComponentInChildren<EnemyTrigger>().Radius;
            //берем масштаб любого измерения
            var scale = towerTransform.localScale.x;
            roadManager.UpdateDangerInRadius(TowersSoles[chosenTower].Center, rad * scale, chosenTower.DangerChangePerLevel);
            _towerInfo.UpdateDamage(chosenTower);
        }

        private void RemoveTower(Tower tower)
        {
            //ищем дороги в радиусе поражения и меняем их опасность
            var towerTransform = tower.transform;
            var rad = towerTransform.GetComponentInChildren<EnemyTrigger>().Radius;
            //берем масштаб любого измерения
            var scale = towerTransform.localScale.x;
            roadManager.UpdateDangerInRadius(TowersSoles[tower].Center, rad * scale, -tower.CurrentDanger);
            _towerInfo.CloseMenu();
            TowersSoles[tower].IsFree = true;
            TowersSoles.Remove(tower);
        }

        public void ShowChosenTower(SolePlatform sole)
        {
            //показываем призрак строения в зависимости от занятости фундамента
            _chosenTower.Init(sole.IsFree &&  _chosenTower.Cost <= moneyManager.Money ? 
                Tower.TowerState.GreenGhost : Tower.TowerState.RedGhost, sole.Center, this, uiManager.AudioVolume);
        }

        public void HideTower()
        {
            _chosenTower.Init(false);
        }

        public void ShowTowersLevels()
        {
            ShowLevel?.Invoke(!_showingLevels);
            _showingLevels = !_showingLevels;
        }

        public void SetLevelTextsRotation(Vector3 v)
        {
            UpdateRotation?.Invoke(v);
        }
        
        public void SellTower(Tower chosenTower)
        {
            chosenTower.Sell();
        }
    }
}
