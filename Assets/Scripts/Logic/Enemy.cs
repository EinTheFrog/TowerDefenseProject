using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Logic
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float speed = 0;
        [SerializeField] private float health = 10;
        [SerializeField] private float levitateHeight = 0;

        private Vector3 _velocity = Vector3.zero;
 
        public Queue<RoadPlatform> Path { get; private set; }
        private EnemyManager _manager;
        public RoadPlatform LastDestination { get; private set; }
        public RoadPlatform NextDestination { get; private set; }
        public bool HasTreasure { get; set; }
        public float ReceivedDamage { get; set; }
        public bool CarriersPath { get; set; }

        public delegate void DieHandler(Enemy enemy);
        public event DieHandler Die;

        public void Init(Queue<RoadPlatform> initPath, EnemyManager manager)
        {
            _manager = manager;
            if (initPath.Count == 0)
            {
                Debug.LogError("Cannot process empty path");
                return;
            }

            Path = new Queue<RoadPlatform>(initPath);
            transform.localPosition = PosAbove(Path.Peek());
            LastDestination = Path.Dequeue();
            //тут каой-то баг и я не могу прямо в выражении задать null, поэтому работаем так
            NextDestination = null;
            NextDestination = Path.Count > 0 ? Path.Peek() : NextDestination;
            if (NextDestination != null)
            {
                CalculateVelocity();
            }
            Die += _manager.Kill;
        }

        private void OnEnable()
        {
            ReceivedDamage = 0;
        }

        private void Update()
        {
            health -= ReceivedDamage * Time.deltaTime;
            if (health <= 0)
            {
                Die(this);
            }
        }

        private void FixedUpdate()
        {
            if (_manager == null) return;

            if (NextDestination != null)
            {
                MovementUpdate();
            } else
            {
                if (HasTreasure)
                {
                    DefeatMenuBehaviour.Instance.Show();
                }
                UpdatePath();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            //Если столкнулись с противником, несущим сокровище, то должны просто следовать зат ним
            if (enemy && enemy.HasTreasure)
            {
                SetPath(enemy.Path);
                CarriersPath = true;
            }
            else
            {
                //Если столкнулись с скоровищем, то должны забрать его
                if (other.GetComponent<Treasure>() != null)
                {
                    _manager.CaptureTreasure(this);
                }
            }
        }

        void MovementUpdate()
        {
            Vector3 distinationPos = PosAbove(NextDestination);
            //Если добрались до следующего пункта пути - пора обновить скорость
            if (Vector3.Distance(transform.localPosition, distinationPos) < speed * Time.deltaTime)
            {
                transform.localPosition = distinationPos;
                Path.Dequeue();
                DefineDestinations();
                if (HasTreasure)
                {
                    _manager.UpdateCarrierPosition(this);
                }
            }
            transform.localPosition += _velocity * Time.deltaTime;
        }

        private void SetPath (Queue<RoadPlatform> newPath)
        {
            //копируем данный путь, чтобы не изменять оригинал
            Path = new Queue<RoadPlatform>(newPath);
            DefineDestinations();
        }

        private void DefineDestinations()
        {
            if (Path.Count == 0)
            {
                LastDestination = NextDestination;
                NextDestination = null;
                return;
            }
            //Если мы поменяли направление пути посередние дороги или просто достаем следующий пункт из пути
            //то присваеваем LastDestinatiom значение NextDestination
            if (NextDestination != null && NextDestination != Path.Peek())
            {
                LastDestination = NextDestination;
            }
            NextDestination = Path.Peek();
            CalculateVelocity();
        }

        private void CalculateVelocity() => _velocity = speed * (PosAbove(NextDestination) - transform.localPosition).normalized;

        //Расчитываем позицию противника, с учетом того, что он левитирует над платформами
        private Vector3 PosAbove(RoadPlatform road) => road.Center + Vector3.up * levitateHeight;

        public void UpdatePath()
        {
            if (_manager == null)
            {
                throw new ArgumentException("Manager hasn't been setted");
            }
            SetPath(_manager.GetPath(this));
        }

        public void MeetTheCarrier()
        {
            if (!CarriersPath && !HasTreasure)
            {
                UpdatePath();
            }
        }
    }
}
