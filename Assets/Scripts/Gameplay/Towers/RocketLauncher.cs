using System;
using Gameplay.Enemies;
using UnityEngine;

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

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
            if (_timeSinceLastFire < timeBetweenFires) return;
            _timeSinceLastFire = 0;
            var newRocket = Instantiate(rocketPrefab);
            var height = GetComponent<MeshRenderer>().bounds.size.y;
            var heightV3 = Vector3.up * height;
            var destinationPos = enemy.NextDestination.Center;
            var damage = basicDamage + damagePerLevel * level;
            newRocket.Init(transform.localPosition + heightV3, destinationPos, damage);
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
