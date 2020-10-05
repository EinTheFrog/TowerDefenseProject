using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float secondsBeforeFirstSpawn = 2;
    [SerializeField] float secondsBetweenSpawn = 2;
    [SerializeField] Enemy enemyPrefab = null;
    [SerializeField] RoadPlatform spawnPlatform = null;
    [SerializeField] RoadPlatform treasurePlatform = null;
    [SerializeField] Treasure treasurePrefab = null;
    [SerializeField] RoadManager roadManager = null;

    float secondsSinceLastSpawn;
    HashSet<Enemy> enemies;
    Treasure treasure;
    Enemy _carrier = null;

    public RoadPlatform ObjectivePlatform { get; private set; }

    //Свойство для хранения противника, несущего сокровище (HasTreasure такого противника должно быть равно true)
    public Enemy Carrier {
        get
        {
            return _carrier;
        }
        set
        {
            _carrier = value;
            if (value)
            {
                _carrier.HasTreasure = true;
            }
            else
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.CarriersPath = false;
                }
            }
        }
    }

    //Свойство для сравнения правильности взаимных ссылок в Manager-ах
    public RoadManager RoadManager { get => roadManager; }

    private void OnValidate()
    {
        if (RoadManager == null ||
            RoadManager.EnemyManager != this)
            Debug.LogError("Reference connection between RoadManger and EnemyManager was broken");
    }

    private void Awake()
    {
        enemies = new HashSet<Enemy>();
        ObjectivePlatform = treasurePlatform;
    }

    private void Start()
    {
        treasure = Instantiate(treasurePrefab);
        treasure.Init(true, ObjectivePlatform.Center);
        //устанавливаем кол-во секунд до следущего спауна (с учетом того, что для спауна secondsSinceLastSpawn должен быть >= secondsBetweenSpawn)
        secondsSinceLastSpawn = secondsBetweenSpawn - secondsBeforeFirstSpawn;
    }

    private void Update()
    {
        if (secondsSinceLastSpawn >= secondsBetweenSpawn)
        {
            secondsSinceLastSpawn -= secondsBetweenSpawn;
            SpawnEnemy();
        }
        secondsSinceLastSpawn += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        //Двигаем сокровище (чтобы не нагружать логикой сокровище)
        if (Carrier != null)
        {
            treasure.SetPosition(Carrier.transform.localPosition);
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.Init(GetPath(spawnPlatform, null, false), this);
        enemies.Add(enemy);
    }

    private Queue<RoadPlatform> GetPath(RoadPlatform lastDestination, RoadPlatform nextDestination, bool hasTreasure)
    {
        RoadPlatform finish;
        //Определяем куда нужно идти противнику
        if (hasTreasure)
        {
            finish = spawnPlatform;
        }
        else if (lastDestination != ObjectivePlatform)
        {
            finish = ObjectivePlatform;
        }
        else
        {
            //если противник дошел до следующей точки маршрута несущего, то он должен двинуться ему навстречу
            finish = Carrier.LastDestination;
        }

        List<RoadPlatform> bestPath;
        //если противник находится на дороге из одного пункта в другой,
        //то он может посередине дороги развернуться и пойти в направлении любого из 2-ух
        if (nextDestination != null)
        {
            List<RoadPlatform> firstPath = roadManager.GetOrAddPath(lastDestination, finish);
            List<RoadPlatform> secondPath = roadManager.GetOrAddPath(nextDestination, finish);
            bestPath = firstPath.Count < secondPath.Count ? firstPath : secondPath;
        }
        else
        {
            bestPath = roadManager.GetOrAddPath(lastDestination, finish);
        }
        return new Queue<RoadPlatform>(bestPath);
    }

    public Queue<RoadPlatform> GetPath(Enemy enemy)
    {
        if (enemy.CarriersPath) return Carrier.Path;

        RoadPlatform next = enemy.NextDestination;
        RoadPlatform last = enemy.LastDestination;

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
        if (treasure.IsCaptured) return;
        treasure.IsCaptured = true;
        Carrier = enemy;
        Carrier.UpdatePath();

        //Все  противники теперь должны двигаться наопережение неусущему
        RoadPlatform oldObjective = ObjectivePlatform;
        ObjectivePlatform = Carrier.NextDestination;
        //Удаляем бесполезный пункт (стоит посреди прямой дороги или вообще ведет в тупик),
        //т.к. он был нужен только для того, чтобы оптимально искать путь до упавшего (или заспауненного) сокровища.
        //Удаляем через oldObjective, чтобы при поиске новых путей при удалении мы пользовались акутальной информацией

        //RemoveNodeIfUseless(oldObjective);
    }

    public void RemoveNodeIfUseless(RoadPlatform road)
    {
        bool shouldBeDeleted = roadManager.ShouldNodeBeDeleted(road);
        if (shouldBeDeleted)
        {
            AwareEnemies(new List<RoadPlatform> { road });
        }
        roadManager.RemoveNode(road);
    }

    public void UpdateCarrierPosition(Enemy caller)
    {
        if (!caller.HasTreasure)
        {
            Debug.LogError("Enemy without treasure is trying to update carriets position");
            return;
        }
        ObjectivePlatform = Carrier.NextDestination;
        //Обновляем позицию до которой (наопережение) нужно двигаться всем проотивникам, которые еще не идут вместе с несущим
        foreach (Enemy enemy in enemies)
        {
            enemy.MeetTheCarrier();
        }
    }

    public void Kill(Enemy dyingEnemy)
    {
        if (dyingEnemy.HasTreasure)
        {
            //Роняем сокровище
            Physics.Raycast(dyingEnemy.transform.localPosition, Vector3.down, out RaycastHit hit, 1f, 1 << 9);
            RoadPlatform road = hit.transform.GetComponent<RoadPlatform>();
            if (road == null) Debug.LogError("Where is no road platform under the enemy!");
            ObjectivePlatform = road;
            treasure.IsCaptured = false;
            //Обнуляем несущего
            enemies.Remove(dyingEnemy);
            Destroy(dyingEnemy.gameObject);
            Carrier = null;
            foreach (Enemy enemy in enemies)
            {
                enemy.UpdatePath();
            }
        } else
        {
            enemies.Remove(dyingEnemy);
            Destroy(dyingEnemy.gameObject);
        }
    }



    //Метод для предупреждения всех необходимых противников о том, что опасность дорог изменилась
    public void AwareEnemies(IEnumerable<RoadPlatform> roads)
    {
        //первым должен обновить путь несущий, чтобы следующие за ним могли просто скопировать его путь
        foreach (RoadPlatform road in roads)
        {
            if (Carrier != null && Carrier.Path.Contains(road))
            {
                Carrier.UpdatePath();
                break;
            }
        }
        foreach (Enemy enemy in enemies)
        {
            foreach (RoadPlatform road in roads)
            {
                if (enemy.Path.Contains(road))
                {
                    enemy.UpdatePath();
                    break;
                }
            }
        }
    }

}
