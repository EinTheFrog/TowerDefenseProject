using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    float movementAcceleration = 10f;
    [SerializeField]
    float movementMaxSpeed = 5f;
    [SerializeField]
    float zoomAcceleration = 6f;
    [SerializeField]
    float zoomMaxSpeed = 2f;
    [SerializeField]
    float rotationAcceleration = 90f;
    [SerializeField]
    float rotationMaxSpeed = 45f;

    Vector3 desiredMovementVelocity;
    Vector3 movementVelocity;

    float desiredRotationVelocity;
    float rotationVelocity;

    float desiredZoomVelocity;
    float zoomVelocity;
    float timeSinceLastScroll;
    Input input;

    public static CameraMovement Instance { get; private set; }
    private void Start()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        input = InputShell.Instance;
        movementVelocity = Vector3.zero;
        rotationVelocity = 0f;
        zoomVelocity = 0f;
        timeSinceLastScroll = 0f;
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


        Vector3 velocity = Vector3.zero;
        velocity += transform.right * movementVelocity.x;
        velocity += transform.forward * zoomVelocity;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.down).normalized;
        velocity += forward * movementVelocity.y;


        float deltaRotation = rotationVelocity * Time.deltaTime;

        transform.localPosition += velocity * Time.deltaTime;
        transform.RotateAround(transform.position + Vector3.forward, Vector3.up, deltaRotation);
    }

    private void ListenKeyboard()
    {
        desiredMovementVelocity = input.MovementMode.Move.ReadValue<Vector2>();
        desiredRotationVelocity = input.MovementMode.Rotate.ReadValue<float>();
    }

    private void ListenMouse()
    {
        float inputScroll = input.MovementMode.Zoom.ReadValue<Vector2>().y;
        if (inputScroll != 0)
        {
            desiredZoomVelocity = Mathf.Sign(inputScroll);
            timeSinceLastScroll = 0f;
        }
        else
        {
            timeSinceLastScroll += Time.deltaTime;
            if (timeSinceLastScroll > 0.1f) desiredZoomVelocity = 0f;
        }
    }

    private void UpdateZoomSpeed()
    {
        float deltaSpeed = zoomAcceleration * Time.deltaTime;
        zoomVelocity = Mathf.Lerp(zoomVelocity, desiredZoomVelocity * zoomMaxSpeed, deltaSpeed);
    }

    private void UpdateMovementSpeed()
    {
        float deltaSpeed = movementAcceleration * Time.deltaTime;
        movementVelocity = Vector3.Lerp(movementVelocity, desiredMovementVelocity * movementMaxSpeed, deltaSpeed);
    }

    private void UpdateRotationSpeed()
    {
        float deltaSpeed = rotationAcceleration * Time.deltaTime;
        rotationVelocity = Mathf.Lerp(rotationVelocity, desiredRotationVelocity * rotationMaxSpeed, deltaSpeed);
    }

    public void TriggerOnTowers(bool isTrigger)
    {
        if (isTrigger)
        {
            GetComponent<PhysicsRaycaster>().eventMask += LayerMask.GetMask("Towers");
        }
        else
        {
            GetComponent<PhysicsRaycaster>().eventMask -= LayerMask.GetMask("Towers");
        }
    }
}