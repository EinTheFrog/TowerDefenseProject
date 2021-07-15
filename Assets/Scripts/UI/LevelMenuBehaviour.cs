using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private UIBehaviour uiBehaviour = default;
        
        public static readonly int LEVELS_AMOUNT = 10;
        private bool[] _levelsStates = new bool[LEVELS_AMOUNT];
        private readonly bool[] _initialLevelsStates = new bool[10] {true, false, false, false, false, false, false, false, false, false};

        private void Start()
        {
            _levelsStates = SaveSystem.SaveSystem.LoadLevelsStates(LEVELS_AMOUNT);
            if (_levelsStates == null)
            {
                _levelsStates = _initialLevelsStates;
                SaveSystem.SaveSystem.SaveLevelsStates(_levelsStates);
            }
            var buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (_levelsStates[i] == false)
                {
                    buttons[i].interactable = false;
                }
            }
        
            uiBehaviour.ChooseLevel(SaveSystem.SaveSystem.MainMenuLevelId);
        }
    }
}
