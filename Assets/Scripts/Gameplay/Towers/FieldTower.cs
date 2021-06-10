using System;
using Gameplay.Enemies;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Towers
{
    public class FieldTower : Tower
    {
        [SerializeField] private FieldAnimation fieldAnimation;
        [SerializeField] private float basicSpeedMultiplier = 1f;
        [SerializeField] private float speedMultiplierPerLevel = 1f;
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

            var speedMultiplier = basicSpeedMultiplier * Mathf.Pow(speedMultiplierPerLevel, Level);
            enemy.Speed *= speedMultiplier;
            EnemiesUnderFire.Add(enemy);
        }

        public override void MoveAim(Enemy enemy)
        {
        }

        public override void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            enemy.RestoreBasicSpeed();;
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
            EnemiesUnderFire.Remove(enemy);
            
            if (EnemiesUnderFire.Count == 0)
            {
                fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStop);
            }
        }
        
        protected override void Update()
        {
            var damage = basicDamage + damagePerLevel * Level;
            foreach (var enemy in EnemiesUnderFire)
            {
                enemy.Health -= damage * Time.deltaTime;
            }
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }
    }
}
