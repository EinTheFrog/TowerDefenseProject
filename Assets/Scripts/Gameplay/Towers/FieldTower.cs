using System;
using Gameplay.Enemies;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Towers
{
    public class FieldTower : Tower
    {
        [SerializeField] private FieldAnimation fieldAnimation;
        [SerializeField] private float basicSpeedDebaff = 0.4f;
        [SerializeField] private float speedDebaffPerLevel = 0.05f;
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
            audio.Play();
            
            //добавляем остановку стрельбы в событие смерти
            enemy.Die += StopShooting;
            enemy.Die += Manager.GetMoneyForKill;

            var speedDebaff = basicSpeedDebaff + speedDebaffPerLevel * level;
            enemy.SlowDown(speedDebaff);
            EnemiesUnderFire.Add(enemy);
        }

        public override void MoveAim(Enemy enemy)
        {
        }

        public override void StopShooting(Enemy enemy)
        {
            if (!EnemiesUnderFire.Contains(enemy)) return;
            
            var speedDebaff = basicSpeedDebaff + speedDebaffPerLevel * level;
            enemy.RestoreBasicSpeed(speedDebaff);;
            enemy.Die -= StopShooting;
            enemy.Die -= Manager.GetMoneyForKill;
            EnemiesUnderFire.Remove(enemy);
            
            if (EnemiesUnderFire.Count == 0)
            {
                fieldAnimation.PlayAnimation(FieldAnimation.Anim.FieldStop);
                audio.Stop();
            }
        }

        protected override void Update()
        {
            var damage = basicDamage + damagePerLevel * level;
            foreach (var enemy in EnemiesUnderFire)
            {
                enemy.Health -= damage * Time.deltaTime;
            }
        }
        
        protected override void UpgradeFeatures()
        {
            var speedDebaff = basicSpeedDebaff + speedDebaffPerLevel * level;
            var oldSpeedDebaff = basicSpeedDebaff + speedDebaffPerLevel * (level - 1);
            foreach (var enemy in EnemiesUnderFire)
            {
                enemy.RestoreBasicSpeed(oldSpeedDebaff);
                enemy.SlowDown(speedDebaff);
            }
        }

        public override string TowerName { protected set; get; } = "Field tower";
    }
}
