using System;
using Gameplay.Enemies;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Towers
{
    public class FieldTower : Tower
    {
        [SerializeField] private FieldAnimation fieldAnimation;
        protected override void Build(Renderer meshRenderer)
        {
            SetBuiltMaterial(meshRenderer);
            fieldAnimation.PlayAnimation(FieldAnimation.Anim.TowerBuild);
        }

        public override void StartShooting(Enemy enemy)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
        
            if (EnemiesUnderFire.Contains(enemy)) return;
            
            fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStart);
            
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
            
            EnemiesUnderFire.Remove(enemy);
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
            
            if (EnemiesUnderFire.Count == 0)
            {
                fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStop);
            }
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }
    }
}
