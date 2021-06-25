using System;
using Gameplay.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class InputShell : MonoBehaviour
    {
        [SerializeField] private CameraMovement cameraMovement = default;
        private Input _input;
        public Mode CurrentMode { get; private set; }

        private void OnEnable()
        {
            _input = new Input();
        }

        public void SetBuildingMode()
        {
            if (_input.BuildMode.enabled) return;
            _input.BuildMode.Enable();
            _input.ViewMode.Disable();
            _input.TowerMode.Disable();
            _input.MenuMode.Disable();
            cameraMovement.TriggerOnTowers(false);

            CurrentMode = Mode.BuildMode;
        }

        public void SetTowerMode()
        {
            if (_input.TowerMode.enabled) return;
            _input.TowerMode.Enable();
            _input.MovementMode.Enable();
            _input.BuildMode.Disable();
            _input.ViewMode.Disable();
            _input.MenuMode.Disable();
            cameraMovement.TriggerOnTowers(true);

            CurrentMode = Mode.TowerMode;
        }

        public void SetViewMode()
        {
            if (_input.ViewMode.enabled) return;
            _input.ViewMode.Enable();
            _input.MovementMode.Enable();
            _input.BuildMode.Disable();
            _input.TowerMode.Disable();
            _input.MenuMode.Disable();
            cameraMovement.TriggerOnTowers(true);

            CurrentMode = Mode.ViewMode;
        }

        public void SetDefeatMode()
        {
            _input.MovementMode.Disable();
            _input.ViewMode.Disable();
            _input.BuildMode.Disable();
            _input.TowerMode.Disable();
            _input.MenuMode.Disable();
            cameraMovement.TriggerOnTowers(false);
            
            CurrentMode = Mode.DefeatMode;
        }
        
        public void SetMenuMode()
        {
            _input.MenuMode.Enable();
            _input.MovementMode.Disable();
            _input.ViewMode.Disable();
            _input.BuildMode.Disable();
            _input.TowerMode.Disable();
            cameraMovement.TriggerOnTowers(false);
            
            CurrentMode = Mode.MenuMode;
        }

        public void SetActionForMode(ActionType actionType, Mode mode, ActionVoid action)
        {
            InputAction inputAction;
            switch (actionType)
            {
                case ActionType.Cancel:
                {
                    switch (mode)
                    {
                        case Mode.BuildMode: inputAction = _input.BuildMode.Cancel;
                            break;
                        case Mode.ViewMode: inputAction = _input.ViewMode.Cancel;
                            break;
                        case Mode.TowerMode: inputAction = _input.TowerMode.Cancel;
                            break;
                        case Mode.MenuMode: inputAction = _input.MenuMode.Cancel;
                            break;
                        default: throw new ArgumentException();
                    } 
                } break;
                case ActionType.ShowAdditionalInfo:
                {
                    switch (mode)
                    {
                        case Mode.BuildMode: inputAction = _input.BuildMode.ShowAdditionalInfo;
                            break;
                        case Mode.ViewMode: inputAction = _input.ViewMode.ShowAdditionalInfo;
                            break;
                        case Mode.TowerMode: inputAction = _input.TowerMode.ShowAdditionalInfo;
                            break;
                        default: throw new ArgumentException();
                    } 
                } break;
                default: throw new ArgumentException();
            }
            inputAction.performed += _ => action();
        }

        public Vector2 ReadMovementInput() => _input.MovementMode.Move.ReadValue<Vector2>();
        
        public float ReadRotationInput() => _input.MovementMode.Rotate.ReadValue<float>();
        
        public Vector2 ReadZoomInput() => _input.MovementMode.Zoom.ReadValue<Vector2>();

        public enum Mode
        {
            ViewMode,
            BuildMode,
            TowerMode,
            MenuMode,
            DefeatMode
        }

        public enum ActionType
        {
            Cancel,
            ShowAdditionalInfo
        }
    }
}

public delegate void ActionVoid();
