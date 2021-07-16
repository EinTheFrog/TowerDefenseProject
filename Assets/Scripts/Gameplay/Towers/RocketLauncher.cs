using System;
using Gameplay.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Towers
{
    public class RocketLauncher : Tower
    {
        [SerializeField] private Rocket rocketPrefab = default;
        [SerializeField] private float timeBetweenFires = default;

        private float _timeSinceLastFire = 0f;

        private void FixedUpdate()
        {
            _timeSinceLastFire += Time.deltaTime;
        }

        protected override void Build(Renderer meshRenderer)
        {
            SetBuiltMaterial(meshRenderer);
        }

        private Vector3 LerpVector3(Vector3 v1, Vector3 v2, float t)
        {
            var x = Mathf.Lerp(v1.x, v2.x, t);
            var y = Mathf.Lerp(v1.y, v2.y, t);
            var z = Mathf.Lerp(v1.z, v2.z, t);

            return new Vector3(x, y, z);
        }

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
            if (_timeSinceLastFire < timeBetweenFires) return;
            _timeSinceLastFire = 0;
            var newRocket = Instantiate(rocketPrefab);
            var height = GetComponent<MeshRenderer>().bounds.size.y;
            var heightV3 = Vector3.up * height;
            var destinationPos = LerpVector3(enemy.LastDestination.Center, enemy.NextDestination.Center, Random.value);
            var damage = basicDamage + damagePerLevel * level;
            newRocket.Init(transform.localPosition + heightV3, destinationPos, damage, _audioVolume);
        }

        public override void MoveAim(Enemy enemy)
        {
            StartShooting(enemy);
        }

        public override void StopShooting(Enemy enemy)
        {
        }
        
        protected override void Update()
        {
        }
    }
}
