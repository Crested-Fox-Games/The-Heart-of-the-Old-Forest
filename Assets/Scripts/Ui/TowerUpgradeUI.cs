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

    [SerializeField]
    private List<TowerUpgradeSO> upgradePathSOs;

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
        foreach (var path in upgradePathSOs)
        {
            var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);

           // slot.GetComponent<TowerSlot>().Initialize(this, path);
        }
    }
}
