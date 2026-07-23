using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementUi : NetworkBehaviour
{
    public static TowerPlacementUi Instance { get; private set; }

    /// <summary>
    /// The prefab for the tower slot UI element
    /// </summary>
    [SerializeField]
    private GameObject towerSlotPrefab;

    /// <summary>
    /// The object that calls the ui 
    /// </summary>
    private GameObject currentTowerSlot;

    [SerializeField]
    private GameObject towerDisplayPanel;

    /// <summary>
    /// The SOs for the towers themselves
    /// </summary>
    [SerializeField]
    private List<TowerSO> towerSOs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        //Destroy children and spawn in new slots for each SO

        //Spawn them back in to ensure that they are all there
        foreach (var tower in towerSOs)
        {
            var slot = Instantiate(towerSlotPrefab, parent: towerDisplayPanel.transform);

            slot.GetComponent<TowerSlot>().Initialize(this, tower);
        }
    }

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
            currentTowerSlot.GetComponent<TempTowerPlacement>().PlaceTower(towerSO);

            //The tower can be placed so we close the ui for tower placement
            UiManager.Instance.HideTowerPlacementUi();
        }

    }

    public void SetCurrentSlot(GameObject currentTower)
    {
        currentTowerSlot = currentTower;
    }
}
