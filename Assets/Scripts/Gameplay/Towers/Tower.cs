﻿using System;
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
        [SerializeField] private int maxLevel = 10;
        [SerializeField] private Material buildingMat = default;
        [SerializeField] private Material greenGhostMat = default;
        [SerializeField] private Material redGhostMat = default;
        [SerializeField] private AudioClip buildSound = default;
        [SerializeField] private float basicDanger = 1;
        [SerializeField] private float dangerChangePerLevel = 1;
        
        public delegate void RemoveHandler(Tower tower);
        public event RemoveHandler Remove;
        
        protected HashSet<Enemy> EnemiesUnderFire;
        protected TowerManager Manager;
        protected int level = 0;
        private GameObject _levelTextObj = default;
        private TextMesh _levelText = default;
        private const string LevelTextTag = "TowerLevelText";
        private TowerMenuBehaviour _towerMenu = default;
        protected AudioSource audio = default;
        private const string ChoseLineTagName = "ChoseLine";
        private LineRenderer _choseLine = default;
        protected float _audioVolume = 1;
        private TowerInfoBehaviour _towerInfo = default;

        public bool IsBuilt { get; private set; }
        public int Cost => cost;
        public int UpgradeCost => upgradeCost;
        public int MaxLevel => maxLevel;
        public int Level => level;
        public float BasicDanger => basicDanger;
        public float DangerChangePerLevel => dangerChangePerLevel;
        public float CurrentDanger => BasicDanger + DangerChangePerLevel * Level;
        public float Range { get; private set; }

        public float Damage => basicDamage + level * damagePerLevel;
        
        public float DamagePerLevel => damagePerLevel;

        private void OnEnable()
        {
            Range = GetComponentInChildren<SphereCollider>().radius * transform.localScale.x;
        }

        private void Start()
        {
            EnemiesUnderFire = new HashSet<Enemy>();
            GetLevelText();
            _towerMenu = FindObjectOfType<TowerMenuBehaviour>().GetComponent<TowerMenuBehaviour>();
            _towerInfo = FindObjectOfType<TowerInfoBehaviour>().GetComponent<TowerInfoBehaviour>();
            _choseLine = FindChildWithTag(ChoseLineTagName).GetComponent<LineRenderer>();
            var pos = transform.position;
            _choseLine.SetPosition(0, pos);
            _choseLine.SetPosition(1, pos);
        }

        private void GetLevelText()
        {
            if (_levelText != null) return;
            _levelTextObj = FindChildWithTag(LevelTextTag);
            _levelText = _levelTextObj.GetComponentInChildren<TextMesh>();
            _levelText.color = Color.clear;
            _levelText.text = level.ToString();
        }

        protected abstract void Update();

        public void OnPointerClick(PointerEventData eventData)
        {
            _towerMenu.CallMenu(this);
            _towerInfo.CallMenu(this);
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
        
        public void Init(TowerState state, Vector3 spawnPos, TowerManager manager, float audioVolume)
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
                    Manager.UpdateRotation += SetRotationByCamPos;
                    GetLevelText();
                    ShowLevel(manager.ShowingLevels);
                    Build(meshRenderer);
                    audio = GetComponent<AudioSource>();
                    audio.PlayOneShot(buildSound, 1f);
                    audio.volume = audioVolume;
                    _audioVolume = audioVolume;
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
            Manager.UpdateRotation -= SetRotationByCamPos;
            
            Destroy(gameObject);
        }

        public abstract void StartShooting(Enemy enemy);

        public abstract void MoveAim(Enemy enemy);

        public abstract void StopShooting(Enemy enemy);

        protected abstract void UpgradeFeatures();
        
        public abstract string TowerName { protected set; get; }

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
            var enemiesCopy = new Enemy[EnemiesUnderFire.Count];
            EnemiesUnderFire.CopyTo(enemiesCopy);
            foreach (var enemy in enemiesCopy)
            {
                StopShooting(enemy);
            }
            Manager.AddMoney(cost + level * upgradeCost);
            Destroy();
        }

        public void Upgrade()
        {
            level++;
            _levelText.text = level.ToString();
            UpgradeFeatures();
        }

        private void SetRotationByCamPos(Vector3 cameraPos)
        {
            _levelTextObj.transform.LookAt(cameraPos);
        }

        private void ShowLevel(bool shouldShow)
        {
            _levelText.color = shouldShow? Color.white : Color.clear;
        }

        private GameObject FindChildWithTag(string tag)
        {
            var children = GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public void ShowChose(bool b)
        {
            var pos = transform.position;
            var pos1 = pos + Vector3.up * 100;
            if (b)
            {
                _choseLine.SetPosition(1, pos1);
            }
            else
            {
                _choseLine.SetPosition(1, pos);
            }
        }
    }
}
