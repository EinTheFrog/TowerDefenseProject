using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    Tower parrent;

    public float Radius { get => GetComponent<SphereCollider>().radius; }
    private void Start()
    {
        parrent = transform.parent.GetComponent<Tower>();
        if (parrent == null) Debug.LogError("EnemyTrigger's parrent is not a tower!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!parrent.IsBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        parrent.StartShooting(enemy);
    }

    private void OnTriggerStay(Collider other) //реагируем на перемещение противников
    {
        if (!parrent.IsBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        parrent.MoveAim(enemy);
    }

    private void OnTriggerExit(Collider other) //реагируем на выход противников из зоны действия лазера
    {
        if (!parrent.IsBuilded) return;
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
        parrent.StopShooting(enemy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}