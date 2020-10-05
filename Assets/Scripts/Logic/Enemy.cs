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

    public delegate void DieHandler(Enemy enemy);
    public event DieHandler Die;

    public void Init(Queue<RoadPlatform> initPath, EnemyManager manager)
    {
        Manager = manager;
        if (initPath.Count == 0)
        {
            Debug.LogError("Cannot process empty path");
            return;
        }

        Path = new Queue<RoadPlatform>(initPath);
        transform.localPosition = PosAbove(Path.Peek());
        LastDestination = Path.Dequeue();
        //тут каой-то баг и я не могу прямо в выражении задать null, поэтому работаем так
        NextDestination = null;
        NextDestination = Path.Count > 0 ? Path.Peek() : NextDestination;
        if (NextDestination != null)
        {
            CalculateVelocity();
        }
        Die += Manager.Kill;
    }

    private void OnEnable()
    {
        ReceivedDamage = 0;
    }

    private void Update()
    {
        health -= ReceivedDamage * Time.deltaTime;
        if (health <= 0)
        {
            Die(this);
        }
    }

    private void FixedUpdate()
    {
        if (Manager == null) return;

        if (NextDestination != null)
        {
            MovementUpdate();
        } else
        {
            if (HasTreasure)
            {
                DefeatMenuBehaviour.Instance.Show();
            }
            UpdatePath();
            //если у противника нет сокровища и он уже подошел к следующей точке маршрута несущего,
            //то он должен двинуться навстречу несущему 
            //(Carrier может быть равен null при определенных обстоятельствах)
            if (NextDestination == null)
            {
                SetPath(Manager.Carrier.LastDestination);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        //Если столкнулись с противником, несущим сокровище, то должны просто следовать зат ним
        if (enemy && enemy.HasTreasure)
        {
            SetPath(enemy.Path);
            CarriersPath = true;
        }
        else
        {
            //Если столкнулись с скоровищем, то должны забрать его
            if (other.GetComponent<Treasure>() != null)
            {
                Manager.CaptureTreasure(this);
            }
        }
    }

    void MovementUpdate()
    {
        Vector3 distinationPos = PosAbove(NextDestination);
        //Если добрались до следующего пункта пути - пора обновить скорость
        if (Vector3.Distance(transform.localPosition, distinationPos) < speed * Time.deltaTime)
        {
            transform.localPosition = distinationPos;
            Path.Dequeue();
            DefineDestinations();
            if (HasTreasure)
            {
                Manager.UpdateCarrierPosition(this);
            }
        }
        transform.localPosition += velocity * Time.deltaTime;
    }

    private void SetPath (Queue<RoadPlatform> newPath)
    {
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
        //Если мы поменяли направление пути посередние дороги или просто достаем следующий пункт из пути
        //то присваеваем LastDestinatiom значение NextDestination
        if (NextDestination != null && NextDestination != Path.Peek())
        {
            LastDestination = NextDestination;
        }
        NextDestination = Path.Peek();
        CalculateVelocity();
    }

    private void CalculateVelocity() => velocity = speed * (PosAbove(NextDestination) - transform.localPosition).normalized;

    //Расчитываем позицию противника, с учетом того, что он левитирует над платформами
    private Vector3 PosAbove(RoadPlatform road) => road.Center + Vector3.up * levitateHeight;

    public void UpdatePath()
    {
        if (Manager == null)
        {
            throw new ArgumentException("Manager hasn't been setted");
        }
        SetPath(Manager.GetPath(this));
    }

    public void MeetTheCarrier()
    {
        if (!CarriersPath)
        {
            UpdatePath();
        }
    }
}
