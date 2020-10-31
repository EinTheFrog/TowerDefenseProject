using System;
using UnityEngine;

namespace Logic.Towers
{
    public class LaserBeam : Tower
    {
        private LineRenderer _lineRenderer;

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
            
            if (IsShooting) return;

            IsShooting = true;
            _lineRenderer.SetPosition(1, enemy.transform.localPosition);
            enemy.ReceivedDamage += damage;
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
            if (EnemiesUnderFire.Contains(enemy))
            {
                _lineRenderer.SetPosition(1, enemy.transform.localPosition);
            }
        }

        public override  void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            IsShooting = false;
            _lineRenderer.SetPosition(1, transform.localPosition);
            enemy.ReceivedDamage = 0;
            EnemiesUnderFire.Remove(enemy);
            enemy.Die -= StopShooting;
        }

        public override void Upgrade()
        {
            throw new NotImplementedException();
        }

        protected override void Build(Renderer meshRenderer)
        {
            SetBuiltMaterial(meshRenderer);
            var transform1 = transform;
            var localPosition = transform1.localPosition;
            _lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            _lineRenderer.SetPosition(0, localPosition + Vector3.up * transform1.localScale.y);
        }
    }
}
