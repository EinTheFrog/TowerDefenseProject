using System.Collections;
using System.Collections.Generic;
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

    public Enemy Carrier {
        get
        {
            return _carrier;
        }
        set
        {
            _carrier = value;
            if (value) value.HasTreasure = true;
        }
    }

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
        if (Carrier != null) treasure.SetPosition(Carrier.transform.localPosition);
    }

    void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.Init(GetPath(spawnPlatform, null, false), this);
        enemies.Add(enemy);
/*        enemy.SetPosition(spawnPlatform.Center);
        enemy.NextRoadNode = spawnPlatform;
        enemy.Manager = this;
        enemies[enemy] = new List<RoadPlatform>();*/
    }

    private Queue<RoadPlatform> GetPath(RoadPlatform lastDestination, RoadPlatform nextDestination, bool hasTreasure)
    {
        RoadPlatform finish;
        if (hasTreasure)
        {
            finish = spawnPlatform;
            if (lastDestination == finish) ;
        }
        else finish = ObjectivePlatform;

        List<RoadPlatform> bestPath;
        if (nextDestination != null)
        {
            List<RoadPlatform> firstPath = roadManager.GetOrAddPath(lastDestination, finish);
            List<RoadPlatform> secondPath = roadManager.GetOrAddPath(nextDestination, finish);
            bestPath = firstPath.Count < secondPath.Count ? firstPath : secondPath;
        }
        else bestPath = roadManager.GetOrAddPath(lastDestination, finish);
        return new Queue<RoadPlatform>(bestPath);
    }

    public Queue<RoadPlatform> GetPath(Enemy enemy)
    {
        return GetPath(enemy.LastDestination, enemy.NextDestination, enemy.HasTreasure);
    }


    public void CaptureTreasure(Enemy enemy)
    {
        if (treasure.IsCaptured) return;
        treasure.IsCaptured = true;
        Carrier = enemy;
        enemy.UpdatePath();
        roadManager.RemoveNodeIfUseless(ObjectivePlatform);
        ObjectivePlatform = enemy.NextDestination;
    }

    public void UpdateCarrierPosition(Enemy caller)
    {
        if (!caller.HasTreasure)
        {
            Debug.LogError("Enemy withour treasure is trying to update carriets position");
            return;
        }
        foreach (Enemy enemy in enemies) if (!enemy.HasTreasure) enemy.MeetTheCarrier();
    }

    public void Kill(Enemy dyingEnemy)
    {
        if (dyingEnemy.HasTreasure)
        {
            Physics.Raycast(dyingEnemy.transform.localPosition, Vector3.down, out RaycastHit hit, 1f, 1 << 9);
            RoadPlatform road = hit.transform.GetComponent<RoadPlatform>();
            if (road == null) Debug.LogError("Where is no road platform under the enemy!");
            ObjectivePlatform = road;
            treasure.IsCaptured = false;
            Carrier = null;
            foreach (Enemy enemy in enemies) enemy.ReplaceCarriersPath();
        }
        enemies.Remove(dyingEnemy);
        Destroy(dyingEnemy.gameObject);
    }

    public void AwareEnemies(RoadPlatform road)
    {
        foreach (Enemy enemy in enemies) if (enemy.Path.Contains(road)) enemy.UpdatePath();
    }
}
