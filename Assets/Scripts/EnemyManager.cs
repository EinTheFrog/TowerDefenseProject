using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    float secondsBetweenSpawn = 2;
    [SerializeField]
    Enemy enemyPrefab = null;
    [SerializeField]
    RoadPlatform spawnPlatform = null;
    [SerializeField]
    RoadPlatform treasurePlatform = null;
    [SerializeField]
    Treasure treasurePrefab = null;

    float secondsSinceLastSpawn = 0;
    List<Enemy> enemies;
    Treasure treasure;
    Dictionary<RoadPlatform, List<List<RoadPlatform>>> paths;

    public Vector3 TreasurePosition { get { return treasure.PositionForEnemies; } }

    public RoadPlatform TreasurePlatform { get; private set; }

    public Enemy Carrier { get; private set; }

    private void Awake()
    {
        enemies = new List<Enemy>();
        paths = new Dictionary<RoadPlatform, List<List<RoadPlatform>>>();
        TreasurePlatform = treasurePlatform;
    }

    private void Start()
    {
        treasure = Instantiate(treasurePrefab);
        treasure.SetPosition(TreasurePlatform.GetCenterUnderPlatform());
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
        if (Carrier != null)
        {
            treasure.SetPosition(Carrier.transform.localPosition);
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.SetPosition(spawnPlatform);
        enemy.NextRoadNode = spawnPlatform;
        enemy.Manager = this;
        enemies.Add(enemy);
    }

    public Queue<RoadPlatform> GetPath(RoadPlatform lastNode, RoadPlatform nextNode, bool hasTreasure)
    {
        RoadPlatform finish;
        if (hasTreasure) finish = spawnPlatform;
        else if (Carrier != null) finish = Carrier.NextRoadNode;
        else finish = TreasurePlatform;

        List<RoadPlatform> bestPath;
        if (lastNode != nextNode)
        {
            List<RoadPlatform> firstPath = GetOrAddPath(lastNode, finish);
            List<RoadPlatform> secondPath = GetOrAddPath(nextNode, finish);
            bestPath = firstPath.Count < secondPath.Count ? firstPath : secondPath;
        }
        else bestPath = GetOrAddPath(nextNode, finish);

        return new Queue<RoadPlatform>(bestPath);
    }

    private List<RoadPlatform> GetOrAddPath(RoadPlatform start, RoadPlatform finish)
    {
        //сохраняем найденные оптимальные пути и используем их заново при необходимости
        //(не вызвывая лишний раз поиск нового пути)
        List<RoadPlatform> currentPath = new List<RoadPlatform>();

        if (paths.ContainsKey(start))
        {
            foreach (List<RoadPlatform> path in paths[start])
            {
                if (path.Contains(finish))
                {
                    int ind = path.IndexOf(finish);
                    currentPath = path.GetRange(0, ind);
                }
            }
        }
        else paths[start] = new List<List<RoadPlatform>>();

        if (currentPath.Count == 0) currentPath = RoadManager.Instance.FindPath(start, finish);

        for (int i = 0; i < paths[start].Count; i++)
        {
            if (currentPath.Contains(paths[start][i].Last()))
            {
                paths[start][i] = currentPath;
                break;
            }
        }
        return currentPath;
    }

    public bool CaptureTreasure(Enemy enemy)
    {
        if (treasure.isCaptured) return false;
        treasure.isCaptured = true;
        Carrier = enemy;
        enemy.UpdatePath(GetPath(enemy.LastRoadNote, enemy.NextRoadNode, true));
        RoadManager.Instance.RemoveNodeIfUseless(TreasurePlatform);
        TreasurePlatform = null;
        return true;
    }


    public void UpdateCarrierPosition(Enemy caller)
    {
        if (!caller.HasTreasure) return;
        foreach (Enemy enemy in enemies) if (!enemy.HasTreasure) enemy.MeetTheCarrier();
    }

    public void Kill(Enemy dyingEnemy)
    {
        if (dyingEnemy.HasTreasure)
        {
            Physics.Raycast(dyingEnemy.transform.localPosition, Vector3.down, out RaycastHit hit, 1f, 1 << 9);
            RoadPlatform road = hit.transform.GetComponent<RoadPlatform>();
            if (road == null) Debug.LogError("Where is no road platform under enemy! Something gone wrong!");
            TreasurePlatform = road;
            treasure.isCaptured = false;
            Carrier = null;
            foreach (Enemy enemy in enemies)
            {
                enemy.ReplaceCarriersPath();
            }
        }
        Destroy(dyingEnemy.gameObject);
    }
}
