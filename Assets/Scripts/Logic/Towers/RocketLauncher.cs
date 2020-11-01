using System;
using UnityEngine;

namespace Logic.Towers
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
            newRocket.Init(transform.localPosition, enemy.transform.localPosition, damage);
        }

        public override void MoveAim(Enemy enemy)
        {
        }

        public override void StopShooting(Enemy enemy)
        {
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }
    }
}
