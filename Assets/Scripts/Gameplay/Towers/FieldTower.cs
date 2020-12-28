using System;
using Gameplay.Enemies;
using UnityEngine;

namespace Gameplay.Towers
{
    public class FieldTower : Tower
    {
        [SerializeField] private FieldAnimation _fieldAnimation;
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
            _fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStart);
        }

        public override void MoveAim(Enemy enemy)
        {
        }

        public override void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            EnemiesUnderFire.Remove(enemy);
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
            
            if (EnemiesUnderFire.Count == 0)
            {
                _field.gameObject.SetActive(false);
                _fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStop);
            }
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }
    }
}
