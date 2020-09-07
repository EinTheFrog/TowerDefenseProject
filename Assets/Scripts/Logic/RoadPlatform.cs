using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RoadPlatform : Platform
{
    private float _danger = 0f;
    public List<Vector3> NeighboursDirs { get; private set; }
    public Dictionary<Vector3, RoadPlatform> Neighbours { get; private set; }

    public RoadManager Manager { get; set; }

    public float Danger { 
        get
        {
            return _danger;
        }
        set
        {
            Manager.UpdateDanger(this, value);
            _danger = value;
        }
    }
    public int Id { get; set; }
    private void Start()
    {
        Neighbours = new Dictionary<Vector3, RoadPlatform>();
        NeighboursDirs = new List<Vector3>();
        CheckDirForNeighbour(Vector3.forward);
        CheckDirForNeighbour(Vector3.right);
        CheckDirForNeighbour(Vector3.back);
        CheckDirForNeighbour(Vector3.left);
    }

    private void CheckDirForNeighbour(Vector3 direction)
    {
        Physics.Raycast(
            transform.position + transform.right / 2 - transform.forward / 2,
            direction,
            out RaycastHit hit, 2f
            );
        if (hit.transform != null && hit.transform.gameObject.GetComponent<RoadPlatform>() != null)
        {
            NeighboursDirs.Add(direction);
            Neighbours[direction] = hit.transform.gameObject.GetComponent<RoadPlatform>();
        }
    }

    public override string ToString()
    {
        return $"id: {Id}, position: {transform.localPosition}, gameobject name: {gameObject.name}";
    }
}
