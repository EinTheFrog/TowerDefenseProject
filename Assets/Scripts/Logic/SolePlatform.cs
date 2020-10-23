using UI;
using UnityEngine.EventSystems;

namespace Logic
{
    public class SolePlatform : Platform, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Input _input;
        public bool IsFree { get; set; }
        public TowerManager TowerManager { get; set; }

        public void Start()
        {
            //изначально при загрузке уровня все фундаменты свободны
            IsFree = true;
            _input = InputShell.Instance;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!_input.BuildingMode.enabled || !IsFree) return;
            TowerManager.BuildChosenTower(this);
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
            if (!_input.BuildingMode.enabled) return;
            //показываем сооружение при наведении курсора на фундамент
            TowerManager.ShowChosenTower(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_input.BuildingMode.enabled) return;
            //прячем показанное сооружение, если пользователь убрал курсор с платформы
            TowerManager.HideTower();
        }
   
    }
}
