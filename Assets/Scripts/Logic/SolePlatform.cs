using Logic.Towers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Logic
{
    public class SolePlatform : Platform, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private InputShell _inputShell;
        public bool IsFree { get; set; }
        public TowerManager TowerManager { get; set; }

        public void Start()
        {
            _inputShell = GameObject.Find("InputShell").GetComponent<InputShell>();
            //изначально при загрузке уровня все фундаменты свободны
            IsFree = true;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (_inputShell.CurrentMode != InputShell.Mode.BuildMode || !IsFree) return;
            if (!TowerManager.BuyChosenTower(this)) return;
            IsFree = false;
            //Вызываем OnPointerEnter, чтобы пользователь сразу после постройки здания видел,
            //что данный фундамент занят и ему не приходилось для этого убирать и
            //сново наводить курсор на эту платформу
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = eventData.position,
                pointerCurrentRaycast = eventData.pointerCurrentRaycast
            };
            OnPointerEnter(pointerEventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_inputShell.CurrentMode != InputShell.Mode.BuildMode) return;
            //показываем сооружение при наведении курсора на фундамент
            TowerManager.ShowChosenTower(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_inputShell.CurrentMode != InputShell.Mode.BuildMode) return;
            //прячем показанное сооружение, если пользователь убрал курсор с платформы
            TowerManager.HideTower();
        }
   
    }
}
