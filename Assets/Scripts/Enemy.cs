using System.Collections;
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
    public EnemyManager Manager { get; set; }
    public RoadPlatform LastRoadNote { get; private set; }
    public RoadPlatform NextRoadNode { get; set; }
    public bool HasTreasure { get; private set; }
    public float ReceivedDamage { get; set; }
    public bool CarriersPath { get; set; }

    private void Start()
    {
        ReceivedDamage = 0;
        LastRoadNote = NextRoadNode;
        UpdatePath(Manager.GetPath(LastRoadNote, NextRoadNode, HasTreasure));
    }

    private void Update()
    {
        health -= ReceivedDamage * Time.deltaTime;
        if (health <= 0) Manager.Kill(this);
    }

    private void FixedUpdate()
    {
        if (Path.Count > 0) MovementUpdate();
        else
        {
            UpdatePath(Manager.GetPath(LastRoadNote, NextRoadNode, HasTreasure));
            if (HasTreasure) Manager.UpdateCarrierPosition(this);
            //если у противника нет сокровища и он уже подошел к следующей точке маршрута несущего,
            //то он должен двинуться навстречу несущему 
            //(Carrier может быть равен null при определенных обстоятельствах)
            else if (Path.Count == 1 && LastRoadNote == NextRoadNode && Manager.Carrier != null)
                MoveTo(Manager.Carrier.LastRoadNote);
        }
    }

    void MovementUpdate()
    {
        if (Vector3.Distance(transform.localPosition, NextRoadNode.GetCenterForHeight(transform.localPosition.y)) < speed * Time.deltaTime)
        {
            transform.localPosition = NextRoadNode.GetCenterForHeight(transform.localPosition.y);
            Path.Dequeue();
            LastRoadNote = NextRoadNode;
            if (Path.Count == 0) return;
            NextRoadNode = Path.Peek();
            if (HasTreasure) Manager.UpdateCarrierPosition(this);
            velocity = speed * (NextRoadNode.GetCenterForHeight(transform.localPosition.y) - transform.localPosition).normalized;
        }
        transform.localPosition += velocity * Time.deltaTime;
    }

    public void MeetTheCarrier()
    {
        if (!CarriersPath) Path = Manager.GetPath(LastRoadNote, NextRoadNode, HasTreasure);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && enemy.HasTreasure)
        {
            UpdatePath(enemy.Path);
            CarriersPath = true;
        }
        else
        {
            if (other.GetComponent<Treasure>() != null)
            {
                HasTreasure = Manager.CaptureTreasure(this);
            }
        }
    }

    public void UpdatePath(Queue<RoadPlatform> newPath)
    {
        Path = new Queue<RoadPlatform>(newPath);
        if (NextRoadNode != Path.Peek())
        {
            LastRoadNote = NextRoadNode;
            NextRoadNode = Path.Peek();
        }
        velocity = 
            speed * 
            (NextRoadNode.GetCenterForHeight(transform.localPosition.y)- transform.localPosition)
            .normalized;
    }

    private void MoveTo(RoadPlatform target)
    {
        Path.Clear();
        Path.Enqueue(target);
        LastRoadNote = NextRoadNode;
        NextRoadNode = target;
        velocity = speed * (NextRoadNode.GetCenterForHeight(transform.localPosition.y) - transform.localPosition).normalized;
    }

    public void SetPosition(RoadPlatform road)
    {
        transform.localPosition = road.GetCenterForHeight(transform.localPosition.y) + Vector3.up * levitateHeight;
    }

    public void ReplaceCarriersPath()
    {
        if (!CarriersPath) return;
        UpdatePath(Manager.GetPath(LastRoadNote, NextRoadNode, HasTreasure));
    }
}
