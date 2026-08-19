using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// A data class used for the dictionary of tower upgrades
/// </summary>
public struct GlobalTowerUpgradesDC
{
    public static GlobalTowerUpgradesDC Default => new GlobalTowerUpgradesDC
    {
        attackAdd = 0f,
        fireRateAdd = 0f,
        healthAdd = 0f,
        rangeAdd = 0f,

        attackMult = 1f,
        fireRateMult = 1f,
        healthMult = 1f,
        rangeMult = 1f,
    };

    //Attack Modifiers
    public float attackAdd;
    public float attackMult;

    //Attack Speed Modifiers
    public float fireRateAdd;
    public float fireRateMult;

    //Health Modifiers
    public float healthAdd;
    public float healthMult;

    //Range Modifiers
    public float rangeAdd;
    public float rangeMult;
}

public class TowerManager : NetworkBehaviour
{
    public static TowerManager Instance { get; private set; }

    /// <summary>
    /// The dictionary that holds the upgrades for all towers
    /// </summary>
    private readonly SyncDictionary<TowerSO, GlobalTowerUpgradesDC> globalTowerUpgrades = new();

    public IReadOnlyDictionary<TowerSO, GlobalTowerUpgradesDC> GlobalTowerUpgrades => globalTowerUpgrades;

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

    public override void OnStartServer()
    {
        base.OnStartServer();

        globalTowerUpgrades.OnChange += ApplyGlobalUpgrades;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        globalTowerUpgrades.OnChange -= ApplyGlobalUpgrades;
    }

    public void AddGlobalUpgrade(TowerSO towerSO, TowerStats towerStat, UpgradeType rewardType, float rewardAmount)
    {
        //Checks to ensure we are running this on the server
        if (!InstanceFinder.IsServerStarted)
            return;

        GlobalTowerUpgradesDC upgrades = GetOrCreateGlobalUpgrades(towerSO);

        //Adds to the upgrades
        if(rewardType == UpgradeType.Addition)
        {
            switch(towerStat)
            {
                case TowerStats.Attack:
                    upgrades.attackAdd += rewardAmount;
                    break;
                case TowerStats.Health:
                    upgrades.healthAdd += rewardAmount;
                    break;
                case TowerStats.FireRate:
                    upgrades.fireRateAdd += rewardAmount;
                    break;
                case TowerStats.Range:
                    upgrades.rangeAdd += rewardAmount;
                    break;
            }
        }
        else
        {
            switch (towerStat)
            {
                case TowerStats.Attack:
                    upgrades.attackMult += rewardAmount;
                    break;
                case TowerStats.Health:
                    upgrades.healthMult += rewardAmount;
                    break;
                case TowerStats.FireRate:
                    upgrades.fireRateMult += rewardAmount;
                    break;
                case TowerStats.Range:
                    upgrades.rangeMult += rewardAmount;
                    break;
            }
        }

        //Updates the upgrades in the dictionary
        globalTowerUpgrades[towerSO] = upgrades;
    }


    /// <summary>
    /// Applies the changes to the upgrades to all relevant towers
    /// </summary>
    /// <param name="towerSO"></param>
    /// <param name="towerStat"></param>
    /// <param name="rewardType"></param>
    /// <param name="rewardAmount"></param>
    private void ApplyGlobalUpgrades(SyncDictionaryOperation op, TowerSO key, GlobalTowerUpgradesDC value, bool asServer)
    {
        //Checks to ensure we are running this on the server
        if (!asServer)
            return;

        //Gets all towers and puts them into an array
        Tower[] towers = FindObjectsByType<Tower>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        //Loops through all towers
        foreach (Tower tower in towers)
        {
            //If the towerSO doesnt match the current tower, skip over this one and continue the loop
            if (tower.TowerSO != key)
                continue;

            //Tell the tower that its upgrade has changed
            tower.OnUpgradesChanged();
        }
    }

    /// <summary>
    /// Either creates or gets the upgrades for a specific tower type
    /// </summary>
    /// <param name="towerSO"></param>
    /// <returns></returns>
    public GlobalTowerUpgradesDC GetOrCreateGlobalUpgrades(TowerSO towerSO)
    {
        //If it cant find the tower upgrades DC then it creates a default one
        if (!globalTowerUpgrades.TryGetValue(towerSO, out GlobalTowerUpgradesDC upgrades))
        {
            upgrades = GlobalTowerUpgradesDC.Default;
            globalTowerUpgrades.Add(towerSO, upgrades);
        }

        //Either returns the created one, or the one from the try get values out
        return upgrades;
    }
}
