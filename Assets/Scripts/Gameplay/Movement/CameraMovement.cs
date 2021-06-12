using Gameplay.Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Movement
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float movementAcceleration = 10f;
        [SerializeField] private float movementMaxSpeed = 5f;
        [SerializeField] private float zoomAcceleration = 6f;
        [SerializeField] private float zoomMaxSpeed = 2f;
        [SerializeField] private float rotationAcceleration = 90f;
        [SerializeField] private float rotationMaxSpeed = 45f;
        [SerializeField] private InputShell inputShell = default;
        [SerializeField] private TowerManager towerManager = default;

        private Vector3 _desiredMovementVelocity;
        private Vector3 _movementVelocity;

        private float _desiredRotationVelocity;
        private float _rotationVelocity;

        private float _desiredZoomVelocity;
        private float _zoomVelocity;
        private float _timeSinceLastScroll;

        private bool _collides = false;
        private void OnEnable()
        {
            _movementVelocity = Vector3.zero;
            _rotationVelocity = 0f;
            _zoomVelocity = 0f;
            _timeSinceLastScroll = 0f;
        }

        private void Update()
        {
            ListenKeyboard();
            ListenMouse();
        }

        private void FixedUpdate()
        {
            UpdateMovementSpeed();
            UpdateZoomSpeed();
            UpdateRotationSpeed();

            var tform = transform;
            var velocity = Vector3.zero;
            velocity += tform.right * _movementVelocity.x;
            var forward = tform.forward;
            velocity += _collides? Vector3.down * _zoomVelocity: forward * _zoomVelocity;
            var vForward = Vector3.ProjectOnPlane(forward, Vector3.down).normalized;
            velocity += vForward * _movementVelocity.y;
            var deltaRotation = _rotationVelocity * Time.deltaTime;
            
            var localPos = tform.localPosition;
            var newPos = localPos + velocity * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, -12f, 12f);
            newPos.z = Mathf.Clamp(newPos.z, -12f, 20f);
            
            tform.localPosition = newPos;
            tform.RotateAround(localPos + Vector3.forward, Vector3.up, deltaRotation);
            towerManager.SetLevelTextsRotation(tform.position);
        }

        private void ListenKeyboard()
        {
            _desiredMovementVelocity = inputShell.ReadMovementInput();
            _desiredRotationVelocity = inputShell.ReadRotationInput();
        }

        private void ListenMouse()
        {
            var inputScroll = inputShell.ReadZoomInput().y;
            if (inputScroll != 0)
            {
                _desiredZoomVelocity = Mathf.Sign(inputScroll);
                _timeSinceLastScroll = 0f;
            }
            else
            {
                _timeSinceLastScroll += Time.deltaTime;
                if (_timeSinceLastScroll > 0.1f) _desiredZoomVelocity = 0f;
            }
        }

        private void UpdateZoomSpeed()
        {
            float deltaSpeed = zoomAcceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Lerp(_zoomVelocity, _desiredZoomVelocity * zoomMaxSpeed, deltaSpeed);
        }

        private void UpdateMovementSpeed()
        {
            float deltaSpeed = movementAcceleration * Time.deltaTime;
            _movementVelocity = Vector3.Lerp(_movementVelocity, _desiredMovementVelocity * movementMaxSpeed, deltaSpeed);
        }

        private void UpdateRotationSpeed()
        {
            float deltaSpeed = rotationAcceleration * Time.deltaTime;
            _rotationVelocity = Mathf.Lerp(_rotationVelocity, _desiredRotationVelocity * rotationMaxSpeed, deltaSpeed);
        }

        public void TriggerOnTowers(bool isTrigger)
        {
            if (isTrigger)
            {
                GetComponentInChildren<PhysicsRaycaster>().eventMask += LayerMask.GetMask("Towers");
            }
            else
            {
                GetComponentInChildren<PhysicsRaycaster>().eventMask -= LayerMask.GetMask("Towers");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            _collides = true;
        }
        
        private void OnCollisionExit(Collision other)
        {
            _collides = false;
        }
    }
}