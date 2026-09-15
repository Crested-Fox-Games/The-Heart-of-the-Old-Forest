using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private Tower subscribedTower;

    [SerializeField]
    private List<TowerUpgradePathSO> towerUpgradePathSOs;

    private int selectedPath;

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

        Tower tower = currentTower.GetComponent<Tower>();

        //Change logic based on if a path is selected
        if (!pathSelected)
        {
            //Spawn all paths if we havent selected one
            for (int i = 0; i < towerUpgradePathSOs.Count; i++)
            {
                var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);

                TowerUpgradeSO upgrade = tower.GetNextUpgrade(i);

                slot.GetComponent<UpgradeSlot>().Initialize(this, upgrade, i);
            }
        }
        else
        {
            //Spawn only the selected path if we have selected one
            var slot = Instantiate(upgradePrefab, parent: towerUpgradePanel.transform);
            
            TowerUpgradeSO upgrade = tower.GetNextUpgrade(selectedPath);

            slot.GetComponent<UpgradeSlot>().Initialize(this, upgrade, selectedPath);
        }

        //Subscribe to the current towers 
        subscribedTower = tower;
        subscribedTower.UpgradeProgress.OnChange += UpdateUpgradeUi;
    }

    private void OnDisable()
    {
        subscribedTower.UpgradeProgress.OnChange -= UpdateUpgradeUi;
    }

    private void UpdateUpgradeUi(SyncDictionaryOperation op, int key, TowerPathProgress value, bool asServer)
    {
        //Handle path not selected logic
        if (!pathSelected)
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

            //Updates to tell us path has been selected
            pathSelected = true;
            selectedPath = key;
        }

        UpgradeSlot currentSlot = GetComponentInChildren<UpgradeSlot>();

        //Updates the ui for the current tower
        currentSlot.Initialize(this, subscribedTower.GetNextUpgrade(selectedPath), key);
    }

    public void SetCurrentTower(NetworkObject tower)
    {
        currentTower = tower;
    }

    public void SelectUpgrade(int pathIndex)
    {
        Debug.Log("Tower Upgrade Ui, Calling RPC");
        PlayerRPCHandler.LocalInstance.CallSelectUpgrade(currentTower, pathIndex);
    }
}
