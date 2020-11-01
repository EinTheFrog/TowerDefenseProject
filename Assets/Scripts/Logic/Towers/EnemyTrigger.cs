using UnityEngine;

namespace Logic.Towers
{
    public class EnemyTrigger : MonoBehaviour
    {
        private Tower _parent;

        public float Radius => GetComponent<SphereCollider>().radius;

        private void Start()
        {
            _parent = transform.parent.GetComponent<Tower>();
            if (_parent == null) Debug.LogError("EnemyTrigger's parrent is not a tower!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_parent.IsBuilt) return;
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
            _parent.StartShooting(enemy);
        }

        private void OnTriggerStay(Collider other) //реагируем на перемещение противников
        {
            if (!_parent.IsBuilt) return;
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
            _parent.MoveAim(enemy);
        }

        private void OnTriggerExit(Collider other) //реагируем на выход противников из зоны действия лазера
        {
            if (!_parent.IsBuilt) return;
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) Debug.LogError("Something gone wrong! Tower has triggered on non enemy object");
            _parent.StopShooting(enemy);
        }
    }
}