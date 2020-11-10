using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class InputShell : MonoBehaviour
    {
        [SerializeField] private CameraMovement cameraMovement = default;
        public Input Input { get; private set; }

        private void OnEnable()
        {
            Input = new Input();
        }

        public void SetBuildingMode()
        {
            if (Input.BuildMode.enabled) return;
            Input.BuildMode.Enable();
            Input.ViewMode.Disable();
            Input.TowerMode.Disable();
            cameraMovement.TriggerOnTowers(false);
        }
        
        public void SetTowerMode()
        {
            if (Input.TowerMode.enabled) return;
            Input.TowerMode.Enable();
            Input.BuildMode.Disable();
            Input.ViewMode.Disable();
            cameraMovement.TriggerOnTowers(false);
        }

        public void SetViewMode()
        {
            if (Input.ViewMode.enabled) return;
            Input.ViewMode.Enable();
            Input.BuildMode.Disable();
            Input.TowerMode.Disable();
            cameraMovement.TriggerOnTowers(true);
        }
    }
    
}
