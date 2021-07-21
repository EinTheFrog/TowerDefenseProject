using System;
using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Towers
{
    public class LaserBeam : Tower
    {
        private LineRenderer _lineRenderer;

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
            
            if (EnemiesUnderFire.Count > 0) return;
            
            _lineRenderer.gameObject.SetActive(true);
            var enemyPos = enemy.transform.localPosition;
            _lineRenderer.SetPosition(1, enemyPos);
            audio.Play();
            //добавляем остановку стрельбы в событие смерти
            enemy.Die += StopShooting;
            enemy.Die += Manager.GetMoneyForKill;
            EnemiesUnderFire.Add(enemy);
        }

        public override void MoveAim(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy))
            {
                StartShooting(enemy);
            }

            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            var enemyPos = enemy.transform.localPosition;
            _lineRenderer.SetPosition(1, enemyPos);
        }

        public override  void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            _lineRenderer.SetPosition(1, transform.localPosition);
            _lineRenderer.gameObject.SetActive(false);
            audio.Stop();

            EnemiesUnderFire.Remove(enemy);
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
        }

        protected override void Update()
        {
            var damage = basicDamage + damagePerLevel * level;
            foreach (var enemy in EnemiesUnderFire)
            {
                enemy.Health -= damage * Time.deltaTime;
            }
        }

        protected override void Build(Renderer meshRenderer)
        {
            SetBuiltMaterial(meshRenderer);
            var thisTransform = transform;
            var localPosition = thisTransform.localPosition;
            var size = GetComponent<MeshRenderer>().bounds.size;
            var towerPeekPos = localPosition + Vector3.up * size.y;
            _lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            _lineRenderer.SetPosition(0, towerPeekPos);
            _lineRenderer.SetPosition(1, towerPeekPos);
        }
        
        protected override void UpgradeFeatures()
        {
        }
        
        public override string TowerName { protected set; get; } = "Laser beam";
    }
}
