using System;
using System.Collections.Generic;
using Gameplay.Enemies;
using Gameplay.Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Towers
{
    public abstract class Tower : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected float basicDamage = 10f;
        [SerializeField] protected float damagePerLevel = 2f;
        [SerializeField] private int cost = 10;
        [SerializeField] private int upgradeCost = 10;
        [SerializeField] private Material buildingMat = default;
        [SerializeField] private Material greenGhostMat = default;
        [SerializeField] private Material redGhostMat = default;

        public delegate void RemoveHandler(Tower tower);
        public event RemoveHandler Remove;
        
        protected HashSet<Enemy> EnemiesUnderFire;
        protected TowerManager Manager;
        protected int Level = 0;
        private TextMesh _levelText = default;
        
        public bool IsBuilt { get; private set; }
        public int Cost => cost;

        private void Start()
        {
            EnemiesUnderFire = new HashSet<Enemy>();
            GetLevelText();
        }

        private void GetLevelText()
        {
            if (_levelText != null) return;
            _levelText = GetComponentInChildren<TextMesh>();
            _levelText.color = Color.clear;
            _levelText.text = Level.ToString();
        }

        protected abstract void Update();

        public void OnPointerClick(PointerEventData eventData)
        {
            var menuBehaviour = FindObjectOfType<TowerMenuBehaviour>().GetComponent<TowerMenuBehaviour>();
            menuBehaviour.CallMenu(this);
        }
        
        public void Init(bool isActive, Vector3? spawnPos = null)
        {
            gameObject.SetActive(isActive);
            if (!isActive) return;
            spawnPos = spawnPos ?? Vector3.zero;
            var thisTransform = transform;
            thisTransform.localPosition = (Vector3) spawnPos;
            GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Remove += DestroyThis;
        }
        
        public void Init(TowerState state, Vector3 spawnPos, TowerManager manager)
        {
            Manager = manager;
            Init(true, spawnPos);

            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                Debug.LogError("There is no MeshRenderer for tower");
                return;
            }

            switch (state)
            {
                case TowerState.GreenGhost: meshRenderer.material = greenGhostMat; break;
                case TowerState.RedGhost: meshRenderer.material = redGhostMat; break;
                case TowerState.Building:
                {
                    Manager.ShowLevel += ShowLevel;
                    GetLevelText();
                    ShowLevel(manager.ShowingLevels);
                    Build(meshRenderer); 
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected abstract void Build(Renderer meshRenderer);
        protected void SetBuiltMaterial(Renderer meshRenderer)
        {
            meshRenderer.material = buildingMat;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            IsBuilt = true;
        }

        private void DestroyThis(Tower tower)
        {
            if (Manager == null) throw new MissingFieldException("TowerManager hasn't been added");
            
            foreach (var enemy in EnemiesUnderFire)
            {
                enemy.Die -= StopShooting;
                enemy.Die -= Manager.GetMoneyForKill;
            }
            Manager.ShowLevel -= ShowLevel;
            
            Destroy(gameObject);
        }

        public abstract void StartShooting(Enemy enemy);

        public abstract void MoveAim(Enemy enemy);

        public abstract void StopShooting(Enemy enemy);

        public enum TowerState
        {
            GreenGhost,
            RedGhost,
            Building
        }

        private void Destroy()
        {
            Remove?.Invoke(this);
        }

        public void Sell()
        {
            Manager.AddMoney(cost);
            Destroy();
        }
        
        public void Upgrade() => _levelText.text = (++Level).ToString();

        private void ShowLevel(bool shouldShow)
        {
            _levelText.color = shouldShow? Color.white : Color.clear;
        }
    }
}
