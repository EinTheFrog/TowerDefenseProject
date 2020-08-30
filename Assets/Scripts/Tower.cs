using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    float damage = 10;

    LineRenderer lineRenderer;
    bool isBuilded = false;
    public Tower Build()
    {
        lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
        lineRenderer.SetPosition(0, transform.localPosition + Vector3.up * transform.localScale.y);
        lineRenderer.SetPosition(1, transform.localPosition);
        isBuilded = true;
        return Instantiate(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        lineRenderer.SetPosition(1, enemy.transform.localPosition);
        enemy.ReceivedDamage = damage;
    }

    private void OnTriggerExit (Collider other)
    {
        if (!isBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        lineRenderer.SetPosition(1, transform.localPosition);
    }
}
