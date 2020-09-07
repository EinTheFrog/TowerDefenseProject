using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float speed = 0;
    [SerializeField]
    float health = 10;
    [SerializeField]
    float levitateHeight = 0;

    Vector3 velocity = Vector3.zero;
 
    public Queue<RoadPlatform> Path { get; private set; }
    public EnemyManager Manager { get; private set; }
    public RoadPlatform LastDestination { get; private set; }
    public RoadPlatform NextDestination { get; private set; }
    public bool HasTreasure { get; set; }
    public float ReceivedDamage { get; set; }
    public bool CarriersPath { get; set; }

    private void Init(bool isActive, Vector3? spawnPos = null)
    {
        gameObject.SetActive(isActive);
        if (!isActive) return;
        SetPosition(spawnPos ?? Vector3.zero);
    }

    public void Init(Queue<RoadPlatform> initPath, EnemyManager manager)
    {
        Manager = manager;
        if (initPath.Count == 0)
        {
            Debug.LogError("Cannot process empty path");
            return;
        }
        Init(true, initPath.Peek().Center);
        Path = new Queue<RoadPlatform>(initPath);
        LastDestination = Path.Dequeue();
        //тут каой-то баг и я не могу прямо в выражении задать null, поэтому работаем так
        NextDestination = null;
        NextDestination = Path.Count > 0 ? Path.Peek() : NextDestination;
        CalculateVelocity();
    }

    public void SetPosition(Vector3 spawnPoint)
    {
        transform.localPosition = PosAbove(spawnPoint);
    }

    private void OnEnable()
    {
        ReceivedDamage = 0;
    }

    private void Update()
    {
        health -= ReceivedDamage * Time.deltaTime;
        if (health <= 0) Manager.Kill(this);
    }

    private void FixedUpdate()
    {
        if (NextDestination != null) MovementUpdate();
        else
        {
            if (HasTreasure) DefeatMenuBehaviour.Instance.Show();
            UpdatePath();
            //если у противника нет сокровища и он уже подошел к следующей точке маршрута несущего,
            //то он должен двинуться навстречу несущему 
            //(Carrier может быть равен null при определенных обстоятельствах)
            if (NextDestination == null) SetPath(Manager.Carrier.LastDestination);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && enemy.HasTreasure)
        {
            SetPath(enemy.Path);
            CarriersPath = true;
        }
        else
        {
            if (other.GetComponent<Treasure>() != null)
            {
                Manager.CaptureTreasure(this);
            }
        }
    }

    void MovementUpdate()
    {
        Vector3 distenationPos = PosAbove(NextDestination.Center);
        if (Vector3.Distance(transform.localPosition, distenationPos) < speed * Time.deltaTime)
        {
            transform.localPosition = distenationPos;
            DefineDestinations();
        }
        transform.localPosition += velocity * Time.deltaTime;
    }

    private void SetPath (Queue<RoadPlatform> newPath)
    {
        if (newPath.Count == 0)
        {
            NextDestination = null;
            return;
        }
        //копируем данный путь, чтобы не изменять оригинал
        Path = new Queue<RoadPlatform>(newPath);
        DefineDestinations();
    }

    private void SetPath(RoadPlatform target)
    {
        Path.Clear();
        Path.Enqueue(target);
        DefineDestinations();
    }

    private void DefineDestinations()
    {
        if (Path.Count == 0)
        {
            NextDestination = null;
            return;
        }
        //Если мы поменяли направление пути посередние дороги, то меняем last и next местами
        if (NextDestination != null && NextDestination != Path.Peek()) LastDestination = NextDestination;
        NextDestination = Path.Count > 0 ? Path.Dequeue() : null;
        if (NextDestination != null) CalculateVelocity();
    }

    private void CalculateVelocity() => velocity = speed * (PosAbove(NextDestination.Center) - transform.localPosition).normalized;

    private Vector3 PosAbove(Vector3 pos) => new Vector3(pos.x, pos.y + levitateHeight, pos.z);

    public void UpdatePath()
    {
        SetPath(Manager.GetPath(this));
    }

    public void MeetTheCarrier()
    {
        if (!CarriersPath) UpdatePath();
    }

    public void ReplaceCarriersPath()
    {
        if (!CarriersPath) return;
        UpdatePath();
    }
}
