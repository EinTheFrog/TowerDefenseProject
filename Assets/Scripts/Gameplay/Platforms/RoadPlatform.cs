using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Platforms
{
    [DisallowMultipleComponent]
    public class RoadPlatform : Platform
    {
        //нужен отдельны лист так как мы в RoadManager обращаемся к индексам 0 и 1
        public List<Vector3> NeighboursDirs { get; private set; }
        public Dictionary<Vector3, RoadPlatform> Neighbours { get; private set; }

        public float cost = 1f;

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
            Physics.Raycast(Center - Vector3.up * Size.y * Scale.y, direction,out RaycastHit hit, Size.x * Scale.x * 0.75f);
            if (hit.transform == null || hit.transform.gameObject.GetComponent<RoadPlatform>() == null) return;
            NeighboursDirs.Add(direction);
            Neighbours[direction] = hit.transform.gameObject.GetComponent<RoadPlatform>();
        }

        public override bool Equals(object other) => other.ToString().Equals(ToString());

        public override int GetHashCode() => Id + 31 * (int) (transform.localPosition.x + 31 * transform.localPosition.z);

        public override string ToString() => $"[[Id: {Id}], [Cost: {cost}], [localPosition: {transform.localPosition}]]";
    }
}
