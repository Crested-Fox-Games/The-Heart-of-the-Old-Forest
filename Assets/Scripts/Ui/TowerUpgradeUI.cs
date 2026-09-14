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

        //Spawn them back in to ensure that they are all there
        foreach (var path in towerUpgradePathSOs)
        {
            var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);

            slot.GetComponent<TowerUpgradePath>().Initialize(this, currentTower.GetComponent<Tower>().GetNextUpgrade(currentTower.GetComponent<Tower>().TowerSO, selectedPath));
        }
    }

    public void SetCurrentTower(NetworkObject currentTower)
    {
        this.currentTower = currentTower;
    }

    public void SelectUpgrade()
    {
        TowerUpgradeSO so = currentTower.GetComponent<Tower>().GetNextUpgrade(currentTower.GetComponent<Tower>().TowerSO, selectedPath);
        TowerStatUpgradeSO statSO = so as TowerStatUpgradeSO;
        if (statSO != null)
        {
            PlayerRPCHandler.LocalInstance.CallSelectUpgrade(currentTower, statSO.towerStat.ToString(), statSO.upgradeType);
        }
    }
}
