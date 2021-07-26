using UnityEngine;

namespace Gameplay.Managers
{
    public class FPSmanager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 40;
        }
    }
}
