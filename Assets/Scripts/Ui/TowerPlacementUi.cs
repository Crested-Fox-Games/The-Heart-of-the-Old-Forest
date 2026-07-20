using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementUi : NetworkBehaviour
{
    /// <summary>
    /// The prefab for the tower slot UI element
    /// </summary>
    [SerializeField]
    private GameObject towerSlotPrefab;

    /// <summary>
    /// The SOs for the towers themselves
    /// </summary>
    [SerializeField]
    private List<TowerSO> towerSOs;

    /// <summary>
    /// This will be called when a tower is selected in the ui
    /// </summary>
    /// <param name="towerSO"></param>
    [ServerRpc]
    public void SelectTower(TowerSO towerSO)
    {
        if (BaseResourceController.Instance.RemoveResources(towerSO.RequiredResources))
        {
            //TODO: Tell TempTowerPlacement to spawn this tower


            //The tower can be placed so we close the ui for tower placement
            UiManager.Instance.HideTowerPlacementUi();
        }

    }
}
