using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : MonoBehaviour
{
    [SerializeField] RoadManager roadManager = null;

    Input input;
    Tower chosenTower;
    Dictionary<Tower, SolePlatform> towersSoles;

    public RoadManager RoadManager { get => roadManager; }


    private void OnEnable()
    {
        //узнаем актуальный input (общий для всех)
        input = InputShell.Instance;
        //добавляем реакции на нажатие клавиш
        input.BuildingMode.Quit.performed += _ => ChooseNone();
    }

    private void Start()
    {
        towersSoles = new Dictionary<Tower, SolePlatform>();
    }

    public void ChooseTower() //метод реагирующий на выбор башни в меню
    {
        //узнаем какая башня была выбрана из скрипта, находяшегося в кнопке
        BtnTowerInfo button = EventSystem.current.currentSelectedGameObject.GetComponent<BtnTowerInfo>();
        //прерываем функцию, сели была выбрана не кнопка
        if (button == null)
        {
            Debug.LogError($"Current chosen object is not a button, but it is: {EventSystem.current.currentSelectedGameObject}");
            return;
        }
        chosenTower = Instantiate(button.ButtonsTowerType);
        chosenTower.Init(false);

        //настравиваем реакцию на клавишы в input-e
        InputShell.SetBuildingMode();
    }

    public void ChooseNone() //метод реагирующий на выход из режима постройки
    {
        //удаляем выбранную юашню (она не видима для пользователя, но находится на сцене)
        Destroy(chosenTower.gameObject);
        chosenTower = null;
        //настравиваем реакцию на клавишы в input-e
        InputShell.SetViewMode();
    }

    public void BuildChosenTower(SolePlatform sole)
    {
        //создаем копию призрака (выбранного строения) в указанном месте
        Tower newTower = Instantiate(chosenTower);
        newTower.Init(Tower.TowerState.Building, sole.Center);
        newTower.Remove += RemoveTower;
        towersSoles[newTower] = sole;
        //ищем дороги в радиусе поражения и меняем их опасность
        roadManager.UpdateDangerInRadius(sole.Center, chosenTower.transform.GetComponentInChildren<EnemyTrigger>().Radius, 1);
    }

    private void RemoveTower(Tower tower)
    {
        towersSoles[tower].IsFree = true;
        towersSoles.Remove(tower);
        roadManager.UpdateDangerInRadius(towersSoles[tower].Center, tower.transform.GetComponentInChildren<EnemyTrigger>().Radius, -1);
    }

    public void ShowChosenTower(SolePlatform sole)
    {
        //показываем призрак строения в зависимости от занятости фундамента
        if (sole.IsFree) chosenTower.Init(Tower.TowerState.GreenGhost, sole.Center);
        else chosenTower.Init(Tower.TowerState.RedGhost, sole.Center);
    }

    public void HideTower()
    {
        chosenTower.Init(false);
    }
}
