using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    /// <summary>
    /// Reference to the current tower 
    /// </summary>
    private NetworkObject currentTower;

    /// <summary>
    /// 
    /// </summary>
    private Tower subscribedTower;

    /// <summary>
    /// List of tower upgrade paths
    /// </summary>
    private List<TowerUpgradePathSO> towerUpgradePathSOs;

    /// <summary>
    /// Stores the selected path
    /// </summary>
    private int selectedPath = -1;

    /// <summary>
    /// A bool that tracks if a path has been selected for the current tower
    /// </summary>
    private bool pathSelected = false;

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

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //Destroy children and spawn in new slots for each SO
        foreach (Transform child in towerUpgradePanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (currentTower == null)
            return;

        Tower tower = currentTower.GetComponent<Tower>();

        towerUpgradePathSOs = tower.TowerSO.UpgradePaths;

        //Change logic based on if a path is selected
        if (!pathSelected)
        {
            //Spawn all paths if we havent selected one
            for (int i = 0; i < towerUpgradePathSOs.Count; i++)
            {
                var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);

                TowerUpgradeSO upgrade = tower.GetNextSelectedUpgrade(i);

                slot.GetComponent<UpgradeSlot>().Initialize(this, upgrade, i);
            }
        }
        else
        {
            //Spawn only the selected path if we have selected one
            var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);
            
            TowerUpgradeSO upgrade = tower.GetNextSelectedUpgrade(selectedPath);

            slot.GetComponent<UpgradeSlot>().Initialize(this, upgrade, selectedPath);
        }

        //Subscribe to the current towers 
        subscribedTower = tower;
        subscribedTower.UpgradeProgress.OnChange -= UpdateUpgradeUi;
        subscribedTower.UpgradeProgress.OnChange += UpdateUpgradeUi;
        Debug.Log("Subscribed to onchange");
    }

    private void OnDisable()
    {
        if(subscribedTower != null)
        {
            subscribedTower.UpgradeProgress.OnChange -= UpdateUpgradeUi;
            subscribedTower = null;
        }
            
    }

    private void UpdateUpgradeUi(SyncDictionaryOperation op, int key, TowerPathProgress value, bool asServer)
    {
        Debug.Log("Update upgrade ui");

        if (asServer)
            return;

        //Handle path not selected logic
        if (pathSelected)
        {
            List<UpgradeSlot> slots = GetComponentsInChildren<UpgradeSlot>().ToList();

            //Deletes slots that havent been selected
            foreach (UpgradeSlot slot in slots)
            {
                if(slot.PathIndex != key)
                {
                    Destroy(slot.gameObject);
                }
            }

            UpgradeSlot currentSlot = GetComponentInChildren<UpgradeSlot>();

            //Updates the ui for the current tower
            currentSlot.Initialize(this, subscribedTower.GetNextSelectedUpgrade(selectedPath), key);
        }
    }

    private void SelectUpgradePath(int key)
    {
        //Updates to tell us path has been selected
        pathSelected = true;
        selectedPath = key;
    }

    public void SetCurrentTower(NetworkObject tower)
    {
        currentTower = tower;
    }

    public void SelectUpgrade(int pathIndex)
    {
        if(!pathSelected)
            SelectUpgradePath(pathIndex);

        PlayerRPCHandler.LocalInstance.CallSelectUpgrade(currentTower, pathIndex);
    }
}
