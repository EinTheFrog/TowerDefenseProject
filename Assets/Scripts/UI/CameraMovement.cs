using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float movementAcceleration = 10f;
        [SerializeField] private float movementMaxSpeed = 5f;
        [SerializeField] private float zoomAcceleration = 6f;
        [SerializeField] private float zoomMaxSpeed = 2f;
        [SerializeField] private float rotationAcceleration = 90f;
        [SerializeField] private float rotationMaxSpeed = 45f;
        [SerializeField] private InputShell inputShell = null;

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

            var velocity = Vector3.zero;
            var transform1 = transform;
            velocity += transform1.right * _movementVelocity.x;
            var forward1 = transform1.forward;
            velocity += _collides? Vector3.down * _zoomVelocity: forward1 * _zoomVelocity;
            var forward = Vector3.ProjectOnPlane(forward1, Vector3.down).normalized;
            velocity += forward * _movementVelocity.y;


            var deltaRotation = _rotationVelocity * Time.deltaTime;

            var transform2 = transform;
            transform2.localPosition += velocity * Time.deltaTime;
            transform.RotateAround(transform2.position + Vector3.forward, Vector3.up, deltaRotation);
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