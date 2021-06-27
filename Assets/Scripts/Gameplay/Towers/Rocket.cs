using System;
using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Towers
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = default;

        private Vector3 _desiredPos;
        private Vector3 _velocity;
        private float _damage;
        private bool _isExploding;

        public void Init(Vector3 curPos, Vector3 destinationPos, float damage)
        {
            var thisTransform = transform;
            thisTransform.localPosition = curPos;
            _desiredPos = destinationPos;
            _velocity = (destinationPos - thisTransform.localPosition).normalized * speed;
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
            _isExploding = true;
            
            var explosionAnim = GetComponent<Animator>();
            var explodeTrigger = Animator.StringToHash("Explode");
            explosionAnim.SetTrigger(explodeTrigger);
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayDelayed(1f/3);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isExploding) return;
            
            var enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Health -= _damage;
            }
        }
    }
}
