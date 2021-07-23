using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Enemies;
using Gameplay.Platforms;
using SaveSystem;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private float secondsBeforeFirstSpawn = 2;
        [SerializeField] private float secondsBetweenSpawn = 2;
        [SerializeField] private Enemy[] enemyPrefabs = null;
        [SerializeField] private RoadPlatform spawnPlatform = null;
        [SerializeField] private RoadPlatform treasurePlatform = null;
        [SerializeField] private Treasure treasurePrefab = null;
        [SerializeField] private RoadManager roadManager = null;
        [SerializeField] private int[] enemiesCount = null;
        [SerializeField] private LevelManager levelManager = null;

        private float _secondsSinceLastSpawn;
        private HashSet<Enemy> _enemies;
        private Treasure _treasure;
        private Enemy _carrier;
        private List<KeyValuePair<Enemy, int>> _enemiesLeft;

        public RoadPlatform ObjectivePlatform { get; private set; }

        //Свойство для хранения противника, несущего сокровище (HasTreasure такого противника должно быть равно true)
        private Enemy Carrier {
            get => _carrier;
            set
            {
                _carrier = value;
                if (value != null)
                {
                    _carrier.HasTreasure = true; 
                }
            }
        }

        //Свойство для сравнения правильности взаимных ссылок в Manager-ах
        public RoadManager RoadManager => roadManager;

        private void OnValidate()
        {
            if (RoadManager == null || RoadManager.EnemyManager != this)
            {
                Debug.Log("Reference connection between RoadManger and EnemyManager was broken");
            }
        }

        private void Start()
        {
            _enemiesLeft = new List<KeyValuePair<Enemy, int>>();
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                if (i >= enemiesCount.Length)
                {
                    throw new ArgumentException("There weren't enough elements in enemiesCount");
                }
                _enemiesLeft.Add(new KeyValuePair<Enemy, int>(enemyPrefabs[i], enemiesCount[i]));
            }
            _enemies = new HashSet<Enemy>();
            ObjectivePlatform = treasurePlatform;
            _treasure = Instantiate(treasurePrefab);
            _treasure.Init(true, ObjectivePlatform.Center);
            //устанавливаем кол-во секунд до следущего спауна (с учетом того, что для спауна secondsSinceLastSpawn должен быть >= secondsBetweenSpawn)
            _secondsSinceLastSpawn = secondsBetweenSpawn - secondsBeforeFirstSpawn;
            CheckEmptyEnemies();
        }

        private void Update()
        {
            if (_secondsSinceLastSpawn >= secondsBetweenSpawn)
            {
                _secondsSinceLastSpawn -= secondsBetweenSpawn;
                SpawnEnemy();
            }
            _secondsSinceLastSpawn += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            //Двигаем сокровище (чтобы не нагружать логикой сокровище)
            if (Carrier != null)
            {
                _treasure.SetPosition(Carrier.transform.localPosition);
            }
        }

        private void CheckEmptyEnemies()
        {
            var removeElements= new List<KeyValuePair<Enemy, int>>();
            foreach (var t in _enemiesLeft)
            {
                if (t.Value <= 0)
                {
                    removeElements.Add(t);
                }
            }

            foreach (var element in removeElements)
            {
                _enemiesLeft.Remove(element);
            }
        }

        private void SpawnEnemy()
        {
            var size = _enemiesLeft.Count;
            if (size == 0) return;

            var ind = Random.Range(0, size);
            var enemyPrefab = _enemiesLeft[ind].Key;
            _enemiesLeft[ind] = new KeyValuePair<Enemy, int>(enemyPrefab, _enemiesLeft[ind].Value - 1);
            if (_enemiesLeft[ind].Value == 0)
            {
                _enemiesLeft.RemoveAt(ind);
            }
            
            var enemy = Instantiate(enemyPrefab);
            enemy.Init(GetPath(spawnPlatform, null, false), this);
            _enemies.Add(enemy);
        }

        private Queue<RoadPlatform> GetPath(RoadPlatform lastDestination, RoadPlatform nextDestination, bool hasTreasure)
        {
            RoadPlatform finish;
            //Определяем куда нужно идти противнику
            if (hasTreasure)
            {
                finish = spawnPlatform;
            }
            else if (lastDestination != ObjectivePlatform || nextDestination != null)
            {
                finish = ObjectivePlatform;
            }
            else
            {
                if (Carrier != null)
                {
                    //если противник дошел до следующей точки маршрута несущего, то он должен двинуться ему навстречу
                    finish = Carrier.LastDestination;
                }
                else
                {
                    throw new System.Exception("Enemy is on treasure");
                }
            
            }
            List<RoadPlatform> bestPath;
            //если противник находится на дороге из одного пункта в другой,
            //то он может посередине дороги развернуться и пойти в направлении любого из 2-ух
            if (nextDestination != null)
            {
                var firstPath = roadManager.FindPath(lastDestination, finish);
                var secondPath = roadManager.FindPath(nextDestination, finish);
                bestPath = firstPath.Count < secondPath.Count ? firstPath : secondPath;
            }
            else
            {
                bestPath = roadManager.FindPath(lastDestination, finish);
            }
            return new Queue<RoadPlatform>(bestPath);
        }

        public Queue<RoadPlatform> GetPath(Enemy enemy)
        {
            var next = enemy.NextDestination;
            var last = enemy.LastDestination;

            //destination не должен быть равен промежуточной дороге, т.к. путь строится исключительно по узловым дорогам
            if (next != null && roadManager.GetNodesForRoadBetween(next) != null)
            {
                var roadsBeside = roadManager.GetNodesForRoadBetween(next).Value;
                next = enemy.LastDestination != roadsBeside.Key ? roadsBeside.Key : roadsBeside.Value;
            }
            else if (roadManager.GetNodesForRoadBetween(last) != null)
            {
                var roadsBeside = roadManager.GetNodesForRoadBetween(last).Value;
                last = enemy.NextDestination != roadsBeside.Key ? roadsBeside.Key : roadsBeside.Value;
            }
            return GetPath(last, next, enemy.HasTreasure);
        }

        public void CaptureTreasure(Enemy enemy)
        {
            if (_treasure.IsCaptured) return;
            _treasure.IsCaptured = true;
            _treasure.transform.position = enemy.transform.position;
            
            Carrier = enemy;
            Carrier.UpdatePath();

            //Все  противники теперь должны двигаться наопережение неусущему
            var oldObjective = ObjectivePlatform;
            ObjectivePlatform = Carrier.NextDestination;
            foreach (Enemy otherEnemy in _enemies)
            {
                otherEnemy.MeetTheCarrier();
            }
            //Удаляем бесполезный пункт (стоит посреди прямой дороги или вообще ведет в тупик),
            //т.к. он был нужен только для того, чтобы оптимально искать путь до упавшего (или заспауненного) сокровища.
            //Удаляем через oldObjective, чтобы при поиске новых путей при удалении мы пользовались акутальной информацией

            RemoveNodeIfUseless(oldObjective);
        }

        private void RemoveNodeIfUseless(RoadPlatform road)
        {
            var shouldBeDeleted = RoadManager.ShouldNodeBeDeleted(road);
            if (!shouldBeDeleted) return;
            AwareEnemies(new List<RoadPlatform> { road });
            roadManager.RemoveNode(road);
        }

        public void UpdateCarrierPosition(Enemy caller)
        {
            if (!caller.HasTreasure)
            {
                Debug.LogError("Enemy without treasure is trying to update carriets position");
                return;
            }
            //Если несущий донес сокровище, но еще не вызвал окончание игры, то передаем всем противникам его последнюю позицию (тк его NextDestination == null)
            ObjectivePlatform = Carrier.NextDestination != null ? Carrier.NextDestination  : Carrier.LastDestination;
            //Обновляем позицию до которой (наопережение) нужно двигаться всем проотивникам, которые еще не идут вместе с несущим
            foreach (Enemy enemy in _enemies)
            {
                enemy.MeetTheCarrier();
            }
        }

        public void Kill(Enemy dyingEnemy)
        {
            if (dyingEnemy.HasTreasure)
            {
                //Роняем сокровище
                Physics.Raycast(dyingEnemy.transform.localPosition, Vector3.down, out RaycastHit hit, 1f, 1 << 14);
                var road = hit.transform.GetComponent<RoadPlatform>();
                if (road == null) Debug.LogError("Where is no road platform under the enemy!");
                ObjectivePlatform = road;
                _treasure.IsCaptured = false;
                //Обнуляем несущего
                _enemies.Remove(dyingEnemy);
                Destroy(dyingEnemy.gameObject);
                Carrier = null;
                //обновляем пути всех противников
                foreach (Enemy enemy in _enemies)
                {
                    enemy.UpdatePath();
                }
            } else
            {
                _enemies.Remove(dyingEnemy);
                Destroy(dyingEnemy.gameObject);
            }

            if (_enemies.Count + _enemiesLeft.Count == 0)
            {
                EndGame(true);
            }

            var deathEffect = Instantiate(dyingEnemy.deathEffect);
            deathEffect.transform.position = dyingEnemy.transform.position;
            deathEffect.Play();
        }



        //Метод для предупреждения всех необходимых противников о том, что опасность дорог изменилась
        public void AwareEnemies(IEnumerable<RoadPlatform> roads)
        {
            //первым должен обновить путь несущий, чтобы следующие за ним могли просто скопировать его путь
            var roadPlatforms = roads as RoadPlatform[] ?? roads.ToArray();
            if (roadPlatforms.Any(road => Carrier != null && Carrier.Path.Contains(road)))
            {
                Carrier.UpdatePath();
            }

            foreach (var enemy in _enemies.Where(enemy => roadPlatforms.Any(road => enemy.Path.Contains(road))))
            {
                enemy.UpdatePath();
            }
        }

        private void OnDrawGizmos()
        {
            if (ObjectivePlatform != null)
            {
                Gizmos.DrawLine(ObjectivePlatform.Center, ObjectivePlatform.Center + Vector3.up * 10);
            }
            if (Carrier == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Carrier.LastDestination.Center, Carrier.LastDestination.Center + Vector3.up * 10);
        }

        public void EndGame(bool playerWon)
        {
            EndMenuBehaviour.Instance.Show(playerWon);
            if (playerWon)
            {
                SaveSystem.SaveSystem.CompleteLevel(levelManager.LevelId);
            }
        }
        
    }
}
