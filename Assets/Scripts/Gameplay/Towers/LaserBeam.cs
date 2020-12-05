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
            _lineRenderer.SetPosition(1, enemy.transform.localPosition);
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
            _lineRenderer.SetPosition(1, enemy.transform.localPosition);
        }

        public override  void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            _lineRenderer.SetPosition(1, transform.localPosition);
            _lineRenderer.gameObject.SetActive(false);
            
            EnemiesUnderFire.Remove(enemy);
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
        }

        public override void Upgrade()
        {
            throw new NotImplementedException();
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
    }
}
