using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data class used for the dictionary of tower upgrades
/// </summary>
[Serializable]
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

    //Projectile Modifiers
    public int projectileCountAdd;
}

public class TowerManager : NetworkBehaviour
{
    public static TowerManager Instance { get; private set; }

    /// <summary>
    /// The dictionary that holds the upgrades for all towers
    /// </summary>
    private readonly SyncDictionary<string, GlobalTowerUpgradesDC> globalTowerUpgrades = new();

    public IReadOnlyDictionary<string, GlobalTowerUpgradesDC> GlobalTowerUpgrades => globalTowerUpgrades;

    private readonly Dictionary<string, TowerPathProgress> upgradeProgress = new();

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
        globalTowerUpgrades[towerSO.TowerName] = upgrades;
    }

    public TowerUpgradeSO GetNextUpgrade(TowerSO towerSO, int pathIndex)
    {
        TowerUpgradePathSO path = towerSO.UpgradePaths[pathIndex];

        TowerPathProgress progress = GetPathProgress(towerSO, pathIndex);

        // Milestone upgrade
        if (IsMilestoneUpgrade(path, progress.upgradeCount))
        {
            if (progress.milestoneUpgradeCount < path.MaxUniqueUpgrades)
            {
                return path.MilestoneUpgrade;
            }
        }

        // Already selected a random upgrade.
        if (progress.pendingUpgradeID != -1)
        {
            return FindUpgradeByID(path, progress.pendingUpgradeID);
        }

        // Select a new random upgrade.
        TowerUpgradeSO randomUpgrade = path.GetRandomUpgrade();

        if (randomUpgrade == null)
        {
            return null;
        }

        progress.pendingUpgradeID = randomUpgrade.UpgradeId;

        string key = GetPathKey(towerSO, pathIndex);

        upgradeProgress[key] = progress;

        return randomUpgrade;
    }

    public void PurchaseUpgrade(TowerSO towerSO, int pathIndex)
    {
        if (!InstanceFinder.IsServerStarted)
        {
            return;
        }

        TowerUpgradePathSO path = towerSO.UpgradePaths[pathIndex];

        TowerUpgradeSO upgrade = GetNextUpgrade(towerSO, pathIndex);

        if (upgrade == null)
        {
            return;
        }

        //Check if max upgrades reached
        

        //Let the upgrade apply itself.
        upgrade.GrantUpgrade(towerSO);

        // Update progression.
        string key = GetPathKey(towerSO, pathIndex);

        TowerPathProgress progress = GetPathProgress(towerSO, pathIndex);

        progress.upgradeCount++;
        progress.pendingUpgradeID = -1;

        if (upgrade == path.MilestoneUpgrade)
        {
            progress.milestoneUpgradeCount++;
        }

        upgradeProgress[key] = progress;
    }

    private string GetPathKey(TowerSO towerSO, int pathIndex)
    {
        return $"{towerSO.TowerName}_{pathIndex}";
    }

    private TowerPathProgress GetPathProgress(TowerSO towerSO, int pathIndex)
    {
        string key = GetPathKey(towerSO, pathIndex);

        if (!upgradeProgress.TryGetValue(key, out TowerPathProgress progress))
        {
            progress = new TowerPathProgress
            {
                upgradeCount = 0,
                pendingUpgradeID = -1
            };

            upgradeProgress.Add(key, progress);
        }

        return progress;
    }

    private bool IsMilestoneUpgrade(TowerUpgradePathSO path, int upgradeCount)
    {
        int cycleLength = path.RandomUpgradesBetweenMilestones + 1;

        return upgradeCount % cycleLength == 0;
    }

    private TowerUpgradeSO FindUpgradeByID(TowerUpgradePathSO path, int upgradeID)
    {
        foreach (TowerUpgradeSO upgrade in path.RandomUpgradePool)
        {
            if (upgrade.UpgradeId == upgradeID)
            {
                return upgrade;
            }
        }

        return null;
    }

    public void AddProjectileUpgrade(TowerSO towerSO, int amount)
    {
        if (!InstanceFinder.IsServerStarted)
        {
            return;
        }

        GlobalTowerUpgradesDC upgrades = GetOrCreateGlobalUpgrades(towerSO);

        upgrades.projectileCountAdd += amount;

        globalTowerUpgrades[towerSO.TowerName] = upgrades;
    }


    /// <summary>
    /// Applies the changes to the upgrades to all relevant towers
    /// </summary>
    /// <param name="towerSO"></param>
    /// <param name="towerStat"></param>
    /// <param name="rewardType"></param>
    /// <param name="rewardAmount"></param>
    private void ApplyGlobalUpgrades(SyncDictionaryOperation op, string key, GlobalTowerUpgradesDC value, bool asServer)
    {
        //Checks to ensure we are running this on the server
        //if (!asServer)
        //    return;

        //Gets all towers and puts them into an array
        Tower[] towers = FindObjectsByType<Tower>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        //Loops through all towers
        foreach (Tower tower in towers)
        {
            //If the towerSO doesnt match the current tower, skip over this one and continue the loop
            if (tower.TowerSO.TowerName != key)
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
        if (!globalTowerUpgrades.TryGetValue(towerSO.TowerName, out GlobalTowerUpgradesDC upgrades))
        {
            upgrades = GlobalTowerUpgradesDC.Default;
            globalTowerUpgrades.Add(towerSO.TowerName, upgrades);
        }

        //Either returns the created one, or the one from the try get values out
        return upgrades;
    }
}
