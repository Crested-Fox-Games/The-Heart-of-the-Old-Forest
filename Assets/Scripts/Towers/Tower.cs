using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerStats
{
    Attack,
    Health,
    FireRate,
    Range
}

public struct TowerUpgradesDC
{
    public static TowerUpgradesDC Default => new TowerUpgradesDC()
    {
        attackAdd = 0f,
        fireRateAdd = 0f,
        healthAdd = 0f,
        rangeAdd = 0f,

        attackMult = 1f,
        fireRateMult = 1f,
        healthMult = 1f,
        rangeMult = 1f,

        projectileCountAdd = 0,
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

public abstract class Tower : NetworkBehaviour
{
    #region SO Fields
    [SerializeField]
    protected TowerSO towerSO;

    protected string towerName, towerDescription;

    protected float attackRange, towerDamage, towerMaxHealth, attackCooldown;

    protected GameObject projectile, displayObject;

    #endregion

    public TowerSO TowerSO => towerSO;

    protected GameObject targetEnemy;

    protected List<GameObject> targets = new List<GameObject>();

    protected bool stunned = false;

    //Upgrades
    private readonly Dictionary<string, TowerPathProgress> upgradeProgress = new();

    private readonly SyncDictionary<string, TowerUpgradesDC> localUpgrades = new();

    //Returns total projectile count of tower shots
    protected int GetProjectileCount()
    {
        TowerUpgradesDC towerUpgrades = GetOrCreateLocalUpgrades(towerSO);

        return 1 + towerUpgrades.projectileCountAdd;
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
        TowerUpgradeSO randomUpgrade;

        if (path.NextRandomUpgrade == null)
        {
            // Select a new random upgrade.
            randomUpgrade = path.GetRandomUpgrade();
        }
        else
        {
            randomUpgrade = path.NextRandomUpgrade;
        }


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

        path.GetRandomUpgrade();
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

        TowerUpgradesDC upgrades = GetOrCreateLocalUpgrades(towerSO);

        upgrades.projectileCountAdd += amount;

        localUpgrades[towerSO.TowerName] = upgrades;
    }

    /// <summary>
    /// Either creates or gets the upgrades for a specific tower type
    /// </summary>
    /// <param name="towerSO"></param>
    /// <returns></returns>
    public TowerUpgradesDC GetOrCreateLocalUpgrades(TowerSO towerSO)
    {
        //If it cant find the tower upgrades DC then it creates a default one
        if (!localUpgrades.TryGetValue(towerSO.TowerName, out TowerUpgradesDC upgrades))
        {
            upgrades = TowerUpgradesDC.Default;
            localUpgrades.Add(towerSO.TowerName, upgrades);
        }

        //Either returns the created one, or the one from the try get values out
        return upgrades;
    }

    /// <summary>
    /// The current health of the tower
    /// </summary>
    protected readonly SyncVar<float> currentHealth = new();

    protected Coroutine attackCoroutine;

    private TowerManager towerManager;

    private SphereCollider towerRangeCollider;

    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth.Value = towerMaxHealth;

        StartCoroutine(GetTowerManager());
    }

    private IEnumerator GetTowerManager()
    {
        while(towerManager == null)
        {
            towerManager = FindFirstObjectByType<TowerManager>();
            yield return null;
        }

        //Creates the sphere around the tower that they can attack in
        towerRangeCollider = gameObject.AddComponent<SphereCollider>();
        towerRangeCollider.radius = GetRange();
        towerRangeCollider.isTrigger = true;

        InitializeValues();
    }

    /// <summary>
    /// Sets the initial values of the tower based on the SO
    /// </summary>
    private void InitializeValues()
    {
        //String
        towerName = towerSO.TowerName;
        towerDescription = towerSO.TowerDescription;

        //Float
        attackRange = towerSO.AttackRange;
        towerDamage = towerSO.TowerDamage;
        towerMaxHealth = towerSO.TowerHealth;
        attackCooldown = towerSO.AttackCooldown;

        //GameObjects
        projectile = towerSO.Projectile;
        displayObject = towerSO.DisplayObject;

        if(towerManager.GlobalTowerUpgrades.ContainsKey(towerSO.TowerName))
        {
            OnUpgradesChanged();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value < 0)
        {
            //TODO: Need to create some sort of broken form 

            //Tells the tower placement that this tower has died
            TempTowerPlacement towerPlacement = GetComponentInParent<TempTowerPlacement>();

            //TODO: Change this when above TODO's are done
            towerPlacement.TowerDestroyed();
        }
    }

    /// <summary>
    /// Updates the range and health as they are set values.
    /// Damage and fire rate are done in realtime in the child scripts
    /// </summary>
    public void OnUpgradesChanged()
    {
        //Updates the range of the tower
        towerRangeCollider.radius = GetRange();

        //TODO: Factor in losing max health(if possible)
        //Updates the health of the tower
        float newMaxHealth = GetHealth();

        float difference = newMaxHealth - towerMaxHealth;

        towerMaxHealth = newMaxHealth;
        currentHealth.Value += difference;
        
    }

    protected float GetDamage()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);
        TowerUpgradesDC localUpgrades = GetOrCreateLocalUpgrades(towerSO);

        return (towerDamage + globalUpgrades.attackAdd + localUpgrades.attackAdd) * (globalUpgrades.attackMult + localUpgrades.attackMult);
    }

    protected float GetFireRate()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);
        TowerUpgradesDC localUpgrades = GetOrCreateLocalUpgrades(towerSO);

        return attackCooldown / ((1f + (globalUpgrades.fireRateAdd + localUpgrades.fireRateAdd) * 0.1f) * (globalUpgrades.fireRateMult + localUpgrades.fireRateMult));
    }

    protected float GetRange()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);
        TowerUpgradesDC localUpgrades = GetOrCreateLocalUpgrades(towerSO);

        return (attackRange + globalUpgrades.rangeAdd + localUpgrades.rangeAdd) * (globalUpgrades.rangeMult + localUpgrades.rangeMult);
    }

    protected float GetHealth()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);
        TowerUpgradesDC localUpgrades = GetOrCreateLocalUpgrades(towerSO);

        return (towerMaxHealth + globalUpgrades.healthAdd + localUpgrades.healthAdd) * (globalUpgrades.healthMult + localUpgrades.healthMult);
    }
}
