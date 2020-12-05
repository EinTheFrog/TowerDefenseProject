using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Towers
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = default;
        [SerializeField] private float explosionRadius = default;
        
        private Vector3 _desiredPos;
        private Vector3 _velocity;
        private float _damage;

        public void Init(Vector3 curPos, Vector3 desiredPos, float damage)
        {
            var thisTransform = transform;
            thisTransform.localPosition = curPos;
            _desiredPos = desiredPos;
            _velocity = (desiredPos - thisTransform.localPosition).normalized * speed;
            _damage = damage;
        }
        private void Update()
        {
            var deltaPos = _velocity * Time.deltaTime;
            if ((transform.localPosition + deltaPos - _desiredPos).magnitude > deltaPos.magnitude)
            {
                transform.localPosition += deltaPos;
            }
            else
            {
                Explode();
            }
           
        }

        private void Explode()
        {
            _velocity = Vector3.zero;
            var collidersInRadius = new Collider[200];
            int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, collidersInRadius);
            for (int i = 0; i < count; i++)
            {
                var enemy = collidersInRadius[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Health -= _damage;
                }
            }
            Destroy(gameObject);
        }
    }
}
