using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    float damage = 10;
    [SerializeField]
    Material buildingdMat = null;
    [SerializeField]
    Material greenGhostMat = null;
    [SerializeField]
    Material redGhostMat = null;

    LineRenderer lineRenderer;
    public delegate void RemoveHandler(Tower tower);
    public event RemoveHandler Remove;
    HashSet<Enemy> enemiesUderFire;

    public bool IsBuilded { get; private set; } = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        Remove(this);
    }

    public void Init(bool isActive, Vector3? spawnPos = null)
    {
        gameObject.SetActive(isActive);
        if (!isActive) return;
        spawnPos = spawnPos != null ? spawnPos : Vector3.zero;
        transform.localPosition = (Vector3)spawnPos + Vector3.up * transform.localScale.y;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Remove += DestroyThis;
        enemiesUderFire = new HashSet<Enemy>();
    }

    private void DestroyThis(Tower tower)
    {
        foreach (Enemy enemy in enemiesUderFire)
        {
            enemy.ReceivedDamage -= damage;
            enemy.Die -= StopShooting;
        }
        Destroy(gameObject);
    }

    public void StartShooting(Enemy enemy)
    {
        lineRenderer.SetPosition(1, enemy.transform.localPosition);
        enemy.ReceivedDamage += damage;
        //добавляем остановку стрельбы в событие смерти
        enemy.Die += StopShooting;
        enemiesUderFire.Add(enemy);
    }

    public void MoveAim(Enemy enemy)
    {
        lineRenderer.SetPosition(1, enemy.transform.localPosition);
    }

    public void StopShooting(Enemy enemy)
    {
        lineRenderer.SetPosition(1, transform.localPosition);
        enemy.ReceivedDamage = 0;
        enemiesUderFire.Remove(enemy);
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
                meshRenderer.material = buildingdMat;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.SetPosition(0, transform.localPosition + Vector3.up * transform.localScale.y);
                lineRenderer.SetPosition(1, transform.localPosition);
                IsBuilded = true;
                break;
            }
    }
    }


    public void SetPosition(Vector3 newPos)
    {
        transform.localPosition = newPos + Vector3.up * transform.localScale.y;
    }

    public enum TowerState
    {
        GreenGhost,
        RedGhost,
        Building
    }
}
