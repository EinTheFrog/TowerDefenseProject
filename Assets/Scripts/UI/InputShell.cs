using UnityEngine;

namespace UI
{
    public class InputShell : MonoBehaviour
    {
        private static readonly Input Input = new Input();
        public static Input Instance { get; } = Input;

        public static void SetBuildingMode()
        {
            Debug.Log("Inside SetBuildingMode");
            Input.BuildingMode.Enable();
            Input.ViewMode.Disable();
            CameraMovement.Instance.TriggerOnTowers(false);
            Debug.Log("Exit SetBuildingMode");
        }

        public static void SetViewMode()
        {
            Input.ViewMode.Enable();
            Input.BuildingMode.Disable();
            CameraMovement.Instance.TriggerOnTowers(true);
        }
    }
}
