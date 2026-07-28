using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementUi : MonoBehaviour
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
    private NetworkObject currentTowerSlot;

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
        foreach(Transform child in towerDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }

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
    public void SelectTower(TowerSO towerSO)
    {
        //Tell the player to spawn this tower
        PlayerRPCHandler.Instance.CallPlaceTower(currentTowerSlot, towerSO.TowerName);
    }

    public void SetCurrentSlot(NetworkObject currentTower)
    {
        currentTowerSlot = currentTower;
    }
}
