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
    private void SelectTower(TowerSO towerSO)
    {
        if (CanAffordTower(towerSO))
        {
            //TODO: Tell TempTowerPlacement to spawn this tower

            //The tower can be placed so we close the ui for tower placement
            //UiManager.Instance.HideTowerPlacementUi();
        }
    }

    /// <summary>
    /// This will check each resource and compare it with the bases resources, if none of them can be afforded, then it returns false
    /// </summary>
    /// <param name="tower"></param>
    /// <returns></returns>
    private bool CanAffordTower(TowerSO tower)
    {
        for (int i = 0; i < tower.RequiredResources.Count; i++)
        {
            if (!BaseResourceController.Instance.CheckEnoughResources(tower.RequiredResources[i].resource, tower.RequiredResources[i].cost))
            {
                return false; 
            }
        }

        return true;
    }
}
