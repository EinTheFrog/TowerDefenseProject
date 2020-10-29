using System;
using System.Collections;
using Logic;
using Logic.Towers;
using NUnit.Framework;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Tests.PlayMode
{
    public class TowerManagerTests
    {

        [UnityTest]
        public IEnumerator BuyTower()
        {
            var inputShellObject = new GameObject("InputShell");
            var inputShell = inputShellObject.AddComponent<InputShell>();
            
            var sole = Object.Instantiate(Resources.Load<GameObject>("Prefabs/SolePlatform")).GetComponent<SolePlatform>();
            var tower = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Tower")).GetComponent<Tower>();
            var treasure =  Object.Instantiate(Resources.Load<GameObject>("Prefabs/Treasure")).GetComponent<Treasure>();
            var road = Object.Instantiate(Resources.Load<GameObject>("Prefabs/RoadPlatform")).GetComponent<RoadPlatform>();
            var text = new GameObject().AddComponent<Text>();
            
            var towerManager = new GameObject().AddComponent<TowerManager>();
            var roadManager = new GameObject().AddComponent<RoadManager>();
            var enemyManager = new GameObject().AddComponent<EnemyManager>();
            var moneyManager = new GameObject().AddComponent<MoneyManager>();

            var o1 = new SerializedObject(towerManager);
            var o2 = new SerializedObject(roadManager);
            var o3 = new SerializedObject(enemyManager);
            var o4 = new SerializedObject(moneyManager);

            o1.FindProperty("roadManager").objectReferenceValue = roadManager;
            o1.FindProperty("moneyManager").objectReferenceValue = moneyManager;
            o1.ApplyModifiedProperties();

            o2.FindProperty("enemyManager").objectReferenceValue = enemyManager;
            o2.ApplyModifiedProperties();
            
            o3.FindProperty("roadManager").objectReferenceValue = roadManager;
            o3.FindProperty("treasurePrefab").objectReferenceValue = treasure;
            o3.FindProperty("treasurePlatform").objectReferenceValue = road;
            o3.ApplyModifiedProperties();

            o4.FindProperty("moneyText").objectReferenceValue = text;
            o4.ApplyModifiedProperties();
            
            yield return null;
            moneyManager.Money += tower.Cost;
            
            towerManager.ChooseTower(tower);
            towerManager.BuyChosenTower(sole);

            Assert.AreEqual(true, towerManager.TowersSoles.ContainsValue(sole));

            yield return null;
        }
    }
}
