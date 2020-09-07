using UnityEngine;

public class Tower : MonoBehaviour
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
    bool isBuilded = false;

    public void Init(bool isActive, Vector3? spawnPos = null)
    {
        gameObject.SetActive(isActive);
        if (!isActive) return;
        spawnPos = spawnPos != null ? spawnPos : Vector3.zero;
        transform.localPosition = (Vector3)spawnPos + Vector3.up * transform.localScale.y;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
                    isBuilded = true;
                    break;
                }
        }
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.localPosition = newPos + Vector3.up * transform.localScale.y;
    }

    private void OnTriggerStay(Collider other) //реагируем на перемещение протв
    {
        if (!isBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        lineRenderer.SetPosition(1, enemy.transform.localPosition);
        enemy.ReceivedDamage = damage;
    }

    private void OnTriggerExit(Collider other) //реагируем на выход противников из зоны действия лазера
    {
        if (!isBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        lineRenderer.SetPosition(1, transform.localPosition);
        enemy.ReceivedDamage = 0;
    }

    public enum TowerState
    {
        GreenGhost,
        RedGhost,
        Building
    }
}
