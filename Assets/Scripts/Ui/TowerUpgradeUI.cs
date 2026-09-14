using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeUI : MonoBehaviour
{
    public static TowerUpgradeUI Instance { get; private set; }

    [SerializeField]
    private GameObject towerUpgradePanel;

    /// <summary>
    /// The prefab for the tower slot UI element
    /// </summary>
    [SerializeField]
    private GameObject upgradePrefab;

    private NetworkObject currentTower;

    [SerializeField]
    private List<TowerUpgradePathSO> towerUpgradePathSOs;

    private int selectedPath;

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
        foreach (Transform child in towerUpgradePanel.transform)
        {
            Destroy(child.gameObject);
        }

        Tower tower = currentTower.GetComponent<Tower>();

        for (int i = 0; i < towerUpgradePathSOs.Count; i++)
        {
            var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);

            TowerUpgradeSO upgrade = tower.GetNextUpgrade(i);

            slot.GetComponent<TowerUpgradePath>().Initialize(this, upgrade, i);
        } 
    }

    public void SetCurrentTower(NetworkObject tower)
    {
        currentTower = tower;
    }

    public void SelectUpgrade(int pathIndex)
    {
        PlayerRPCHandler.LocalInstance.CallSelectUpgrade(currentTower, pathIndex);
    }
}
