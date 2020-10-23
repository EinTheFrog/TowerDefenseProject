using UnityEngine;

namespace Logic
{
    public class Treasure : MonoBehaviour
    {
        [SerializeField] private float deceleration = 0;
        [SerializeField] private float levitateHeight = 0;
        public float Deceleration => deceleration;

        public bool IsCaptured { get; set; }

        public Vector3 PositionForEnemies
        {
            get
            {
                var transform1 = transform;
                var localPosition = transform1.localPosition;
                return new Vector3(
                    localPosition.x,
                    localPosition.y - (transform1.localScale.y * 0.5f + levitateHeight),
                    localPosition.z);
            }
        }

        public void Init(bool isActive, Vector3? spawnPos = null)
        {
            if (!isActive) return;
            gameObject.SetActive(true);
            spawnPos = spawnPos ?? Vector3.zero;
            SetPosition((Vector3)spawnPos);
        }

        public void SetPosition(Vector3 newPosition)
        {
            var transform1 = transform;
            var localPosition = newPosition;
            localPosition += Vector3.up * (transform1.localScale.y * 0.5f + levitateHeight);
            transform1.localPosition = localPosition;
        }
    
    }
}
