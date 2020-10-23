using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Tower : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float damage = 10;
    [SerializeField] private Material buildingMat = null;
    [SerializeField] private Material greenGhostMat = null;
    [SerializeField] private Material redGhostMat = null;

    private LineRenderer _lineRenderer;
    public delegate void RemoveHandler(Tower tower);
    public event RemoveHandler Remove;
    private HashSet<Enemy> _enemiesUnderFire;
    private bool _isShooting;

    public bool IsBuilded { get; private set; } = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        Remove?.Invoke(this);
    }

    public void Init(bool isActive, Vector3? spawnPos = null)
    {
        gameObject.SetActive(isActive);
        if (!isActive) return;
        spawnPos = spawnPos ?? Vector3.zero;
        var transform1 = transform;
        transform1.localPosition = (Vector3)spawnPos + Vector3.up * transform1.localScale.y;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Remove += DestroyThis;
        _enemiesUnderFire = new HashSet<Enemy>();
        _isShooting = false;
    }

    private void DestroyThis(Tower tower)
    {
        foreach (var enemy in _enemiesUnderFire)
        {
            enemy.ReceivedDamage -= damage;
            enemy.Die -= StopShooting;
        }
        Destroy(gameObject);
    }

    public void StartShooting(Enemy enemy)
    {
        if (_isShooting) return;

        _isShooting = true;
        _lineRenderer.SetPosition(1, enemy.transform.localPosition);
        enemy.ReceivedDamage += damage;
        //добавляем остановку стрельбы в событие смерти
        enemy.Die += StopShooting;
        _enemiesUnderFire.Add(enemy);
    }

    public void MoveAim(Enemy enemy)
    {
        if (!_enemiesUnderFire.Contains(enemy))
        {
            StartShooting(enemy);
        }
        if (_enemiesUnderFire.Contains(enemy))
        {
            _lineRenderer.SetPosition(1, enemy.transform.localPosition);
        }
    }

    public void StopShooting(Enemy enemy)
    {
        if (!_enemiesUnderFire.Contains(enemy)) return;
        _isShooting = false;
        _lineRenderer.SetPosition(1, transform.localPosition);
        enemy.ReceivedDamage = 0;
        _enemiesUnderFire.Remove(enemy);
        enemy.Die -= StopShooting;
    }

    public void Init(TowerState state, Vector3 spawnPos)
    {
    Init(true, spawnPos);

    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
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
                meshRenderer.material = buildingMat;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                _lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
                _lineRenderer.SetPosition(0, transform.localPosition + Vector3.up * transform.localScale.y);
                _lineRenderer.SetPosition(1, transform.localPosition);
                IsBuilded = true;
                break;
            }
        default:
            throw new ArgumentOutOfRangeException(nameof(state), state, null);
    }
    }


    public void SetPosition(Vector3 newPos)
    {
        var transform1 = transform;
        transform1.localPosition = newPos + Vector3.up * transform1.localScale.y;
    }

    public enum TowerState
    {
        GreenGhost,
        RedGhost,
        Building
    }
}
