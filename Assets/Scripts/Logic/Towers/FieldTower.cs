using System;
using UnityEngine;

namespace Logic.Towers
{
    public class FieldTower : Tower
    {
        private FieldBehaviour _field;
        protected override void Build(Renderer meshRenderer)
        {
            SetBuiltMaterial(meshRenderer);
            _field = GetComponentInChildren<FieldBehaviour>(true).GetComponent<FieldBehaviour>();
        }

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
        
            if (EnemiesUnderFire.Contains(enemy)) return;
        
            _field.gameObject.SetActive(true);
            //добавляем остановку стрельбы в событие смерти
            enemy.Die += StopShooting;
            enemy.Die += Manager.GetMoneyForKill;
            EnemiesUnderFire.Add(enemy);
        }

        public override void MoveAim(Enemy enemy)
        {
        }

        public override void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            if (EnemiesUnderFire.Count == 0)
            {
                _field.gameObject.SetActive(false);
            }
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }
    }
}
