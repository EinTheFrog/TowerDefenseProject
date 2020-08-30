using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    float movementAcceleration = 10;
    [SerializeField]
    float movementMaxSpeed = 5;
    [SerializeField]
    float zoomAcceleration = 6;
    [SerializeField]
    float zoomMaxSpeed = 2;
    [SerializeField]
    float rotationAcceleration = 90;
    [SerializeField]
    float rotationMaxSpeed = 45;

    Vector3 velocity;
    Dictionary<InputType, float> inputSpeed;
    HashSet<InputType> activatedInput;
    float timeSinceLastScroll;
    UnityEngine.InputSystem.Keyboard currenKeyboard = UnityEngine.InputSystem.Keyboard.current;
    UnityEngine.InputSystem.Mouse currentMouse = UnityEngine.InputSystem.Mouse.current;
    private void OnEnable()
    {
        velocity = Vector3.zero;

        inputSpeed = new Dictionary<InputType, float>
        {
            { InputType.W, 0f },
            { InputType.A, 0f },
            { InputType.S, 0f },
            { InputType.D, 0f },
            { InputType.Q, 0f },
            { InputType.E, 0f },
            { InputType.ScrollUp, 0f },
            { InputType.ScrollDown, 0f }
        };

        activatedInput = new HashSet<InputType>();

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

        velocity = Vector3.zero;
        velocity += transform.right * (inputSpeed[InputType.D] - inputSpeed[InputType.A]);
        velocity += transform.forward * (inputSpeed[InputType.ScrollUp] - inputSpeed[InputType.ScrollDown]);
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.down).normalized;
        velocity +=  forward * (inputSpeed[InputType.W] - inputSpeed[InputType.S]);
       

        float deltaRotation = (inputSpeed[InputType.E] - inputSpeed[InputType.Q]) * Time.deltaTime; ;

        transform.localPosition += velocity * Time.deltaTime;
        transform.RotateAround(transform.position + Vector3.forward, Vector3.up, deltaRotation);

        //test
/*        if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
        {
            Physics.Raycast(transform.localPosition, transform.forward, out RaycastHit hitInfo);
            if (hitInfo.transform != null) Debug.Log(hitInfo.transform.localPosition);
        }*/
           
    }

    private void ListenKeyboard()
    {
        if (currenKeyboard.wKey.wasPressedThisFrame) activatedInput.Add(InputType.W);
        if (currenKeyboard.aKey.wasPressedThisFrame) activatedInput.Add(InputType.A);
        if (currenKeyboard.sKey.wasPressedThisFrame) activatedInput.Add(InputType.S);
        if (currenKeyboard.dKey.wasPressedThisFrame) activatedInput.Add(InputType.D);
        if (currenKeyboard.qKey.wasPressedThisFrame)
            activatedInput.Add(InputType.Q);
        if (currenKeyboard.eKey.wasPressedThisFrame) activatedInput.Add(InputType.E);

        if (currenKeyboard.wKey.wasReleasedThisFrame) activatedInput.Remove(InputType.W);
        if (currenKeyboard.aKey.wasReleasedThisFrame) activatedInput.Remove(InputType.A);
        if (currenKeyboard.sKey.wasReleasedThisFrame) activatedInput.Remove(InputType.S);
        if (currenKeyboard.dKey.wasReleasedThisFrame) activatedInput.Remove(InputType.D);
        if (currenKeyboard.qKey.wasReleasedThisFrame) activatedInput.Remove(InputType.Q);
        if (currenKeyboard.eKey.wasReleasedThisFrame) activatedInput.Remove(InputType.E);
    }

    private void ListenMouse()
    {
        if (currentMouse.scroll.ReadValue().y > 0)
        {
            timeSinceLastScroll = 0f;
            activatedInput.Remove(InputType.ScrollDown);
            activatedInput.Add(InputType.ScrollUp);
            return;
        }

        if (currentMouse.scroll.ReadValue().y < 0)
        {
            timeSinceLastScroll = 0f;
            activatedInput.Remove(InputType.ScrollUp);
            activatedInput.Add(InputType.ScrollDown);
            return;
        }

        timeSinceLastScroll += Time.deltaTime;
        if (timeSinceLastScroll > 0.3f)
        {
            activatedInput.Remove(InputType.ScrollDown);
            activatedInput.Remove(InputType.ScrollUp);
        }
    }

    private void UpdateZoomSpeed()
    {
        float deltaZoom = zoomAcceleration * Time.deltaTime;

        UpdateInputSpeed(InputType.ScrollUp, deltaZoom, zoomMaxSpeed);
        UpdateInputSpeed(InputType.ScrollDown, deltaZoom, zoomMaxSpeed);
    }

    private void UpdateMovementSpeed()
    {
        float deltaSpeed = movementAcceleration * Time.deltaTime;

        UpdateInputSpeed(InputType.W, deltaSpeed, movementMaxSpeed);
        UpdateInputSpeed(InputType.A, deltaSpeed, movementMaxSpeed);
        UpdateInputSpeed(InputType.S, deltaSpeed, movementMaxSpeed);
        UpdateInputSpeed(InputType.D, deltaSpeed, movementMaxSpeed);
    }

    private void UpdateRotationSpeed()
    {
        float deltaSpeed = rotationAcceleration * Time.deltaTime;
        UpdateInputSpeed(InputType.Q, deltaSpeed, rotationMaxSpeed);
        UpdateInputSpeed(InputType.E, deltaSpeed, rotationMaxSpeed);
    }

    private void UpdateInputSpeed(InputType inputName, float deltaSpeed, float maxSpeed)
    {
        if (activatedInput.Contains(inputName))
            inputSpeed.ChangeSpeed(inputName, maxSpeed, deltaSpeed);
        else
            inputSpeed.ChangeSpeed(inputName, maxSpeed, -deltaSpeed);
    }
}

public static class MyDictionaryExtensions
{
    public static void ChangeSpeed(
        this Dictionary<InputType,float> dictionary,
        InputType inputName,
        float maxSpeed,
        float deltaSpeed
        )
    {
        float newSpeed = dictionary[inputName] + deltaSpeed < 0 ? 0 : dictionary[inputName] + deltaSpeed;
        dictionary[inputName] = newSpeed > maxSpeed ? dictionary[inputName] : newSpeed;
    }
}

public enum InputType
{
    W, A, S, D, Q, E, ScrollUp, ScrollDown
}