using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField]
    Material buildedMaterial = null;
    [SerializeField]
    Material canBeBuildedMaterial = null;
    [SerializeField]
    Material cannotBeBuildedMaterial = null;
    [SerializeField]
    List<Tower> towers = null;

    Input input;

    private void OnEnable()
    {
        Instance = this;
        input = InputShell.Instance;
        input.BuilderMode.Quit.performed += _ => ChooseNone();
    }
    public static Tower ChosenTower { get; private set; }

    public static TowerManager Instance { get; private set; }
    public void ChooseTower()
    {
        ChosenTower = Instantiate(towers[0]);
        ChosenTower.gameObject.SetActive(false);
        input.BuilderMode.Enable();
        input.ViewerMode.Disable();
    }

    public void ChooseNone()
    {
        if (ChosenTower == null) return;
        Destroy(ChosenTower.gameObject);
        ChosenTower = null;
        input.BuilderMode.Disable();
        input.ViewerMode.Enable();
    }

    public Tower BuildChosenTower(Vector3 buildPoint)
    {
        if (ChosenTower == null) return null;
        ChosenTower.gameObject.SetActive(true);
        ChosenTower.transform.localPosition =
            buildPoint + Vector3.up * ChosenTower.transform.localScale.y;
        ChosenTower.GetComponent<MeshRenderer>().material = buildedMaterial;
        Tower buildedTower = ChosenTower;
        ChosenTower = ChosenTower.Build();
        return buildedTower;
    }

    public void ShowChosenTower(Vector3 buildPoint, bool placeIsFree)
    {
        if (ChosenTower == null) return;
        if (!ChosenTower.gameObject.activeSelf)
        {
            ChosenTower.gameObject.SetActive(true);
            ChosenTower.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        if (placeIsFree) ChosenTower.GetComponent<MeshRenderer>().material = canBeBuildedMaterial;
        else ChosenTower.GetComponent<MeshRenderer>().material = cannotBeBuildedMaterial;
        ChosenTower.transform.localPosition =
            buildPoint + Vector3.up * ChosenTower.transform.localScale.y;
    }

    public void HideTower(Tower tower)
    {
        if (tower == null) return;
        tower.gameObject.SetActive(false);
    }
}
