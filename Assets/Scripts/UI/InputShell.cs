using System;
using UnityEngine;

namespace UI
{
    public class InputShell : MonoBehaviour
    {
        public Input Input { get; private set; }

        private void OnEnable()
        {
            Input = new Input();
        }

        public void SetBuildingMode()
        {
            Input.BuildMode.Enable();
            Input.ViewMode.Disable();
            CameraMovement.Instance.TriggerOnTowers(false);
        }

        public void SetViewMode()
        {
            Input.ViewMode.Enable();
            Input.BuildMode.Disable();
            CameraMovement.Instance.TriggerOnTowers(true);
        }
    }
    
}
