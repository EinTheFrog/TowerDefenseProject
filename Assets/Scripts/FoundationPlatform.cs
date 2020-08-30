using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class FoundationPlatform : Platform, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    Tower buildedTower = null;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (buildedTower != null) return;
        buildedTower = TowerManager.Instance.BuildChosenTower(
            GetCenterUnderPlatform()
            );
        //Вызываем OnPointerEnter, чтобы пользователь сразу после постройки здания видел,
        //что данный фундамент занят и ему не приходилось для этого убирать и
        //сново наводить курсор на эту платформу
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = eventData.position;
        pointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        OnPointerEnter(pointerEventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TowerManager.Instance.ShowChosenTower(
            GetCenterUnderPlatform(),
            buildedTower == null
            );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TowerManager.Instance.HideTower(TowerManager.ChosenTower);
    }
   
}
