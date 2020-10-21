using System.Collections;
using Logic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TowerManagerTests
    {

        [UnityTest]
        public IEnumerator BuildTower()
        {
            Debug.Log("test");
            var soleObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/SolePlatform"));
            var sole = soleObject.GetComponent<SolePlatform>();

            var managerObject = new GameObject();
            var towerManager = managerObject.AddComponent<TowerManager>();
            towerManager.ChooseTower(Resources.Load<GameObject>("Prefabs/Tower").GetComponent<Tower>());
            towerManager.BuildChosenTower(sole);

            yield return null;
            
            Debug.Log(towerManager.towersSoles);
            
            Assert.Equals(true, towerManager.towersSoles.ContainsValue(sole));
        }
    }
}
