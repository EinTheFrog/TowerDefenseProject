using System;
using System.Collections;
using Logic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.PlayMode
{
    public class TowerManagerTests
    {

        [UnityTest]
        public IEnumerator BuildTower()
        {
            var soleObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/SolePlatform"));
            var sole = soleObject.GetComponent<SolePlatform>();
            var towerManagerObject = new GameObject();
            var towerManager = towerManagerObject.AddComponent<TowerManager>();
            var towerObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Tower"));
            var tower = towerObject.GetComponent<Tower>();
            var roadManagerObject = new GameObject();
            var roadManager = roadManagerObject.AddComponent<RoadManager>();
            var enemyManagerObject = new GameObject();
            var enemyManager = enemyManagerObject.AddComponent<EnemyManager>();
            var o1 = new SerializedObject(towerManager);
            var o2 = new SerializedObject(roadManager);
            var o3 = new SerializedObject(enemyManager);
            o1.FindProperty("roadManager").objectReferenceValue = roadManager;
            o1.ApplyModifiedProperties();
            o2.FindProperty("enemyManager").objectReferenceValue = enemyManager;
            o2.ApplyModifiedProperties();
            o3.FindProperty("roadManager").objectReferenceValue = roadManager;
            o3.FindProperty("treasurePrefab").objectReferenceValue =
                Object.Instantiate(Resources.Load<GameObject>("Prefabs/Treasure")).GetComponent<Treasure>();
            o3.FindProperty("treasurePlatform").objectReferenceValue =
                Object.Instantiate(Resources.Load<GameObject>("Prefabs/RoadPlatform")).GetComponent<RoadPlatform>();
            o3.ApplyModifiedProperties();
            yield return null;
            towerManager.ChooseTower(tower);
            towerManager.BuildChosenTower(sole);

            Assert.AreEqual(true, towerManager.TowersSoles.ContainsValue(sole));

            yield return null;
        }
    }
}
