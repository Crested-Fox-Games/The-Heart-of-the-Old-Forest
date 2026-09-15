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
    private readonly Dictionary<int, TowerPathProgress> upgradeProgress = new();

    private TowerUpgradesDC localUpgrades = TowerUpgradesDC.Default;

    public void AddLocalUpgrade(TowerStats towerStat, UpgradeType upgradeType, float upgradeAmount)
    {
        //Checks to ensure we are running this on the server
        if (!InstanceFinder.IsServerStarted)
            return;

        
        //Adds to the upgrades
        if (upgradeType == UpgradeType.Addition)
        {
            switch (towerStat)
            {
                case TowerStats.Attack:
                    localUpgrades.attackAdd += upgradeAmount;
                    break;
                case TowerStats.Health:
                    localUpgrades.healthAdd += upgradeAmount;
                    break;
                case TowerStats.FireRate:
                    localUpgrades.fireRateAdd += upgradeAmount;
                    break;
                case TowerStats.Range:
                    localUpgrades.rangeAdd += upgradeAmount;
                    break;
            }
        }
        else
        {
            switch (towerStat)
            {
                case TowerStats.Attack:
                    localUpgrades.attackMult += upgradeAmount;
                    break;
                case TowerStats.Health:
                    localUpgrades.healthMult += upgradeAmount;
                    break;
                case TowerStats.FireRate:
                    localUpgrades.fireRateMult += upgradeAmount;
                    break;
                case TowerStats.Range:
                    localUpgrades.rangeMult += upgradeAmount;
                    break;
            }
        }

        OnUpgradesChanged();
    }

    //Returns total projectile count of tower shots
    protected int GetProjectileCount()
    {
        return 1 + localUpgrades.projectileCountAdd;
    }

    public TowerUpgradeSO GetNextUpgrade(int pathIndex)
    {
        TowerUpgradePathSO path = towerSO.UpgradePaths[pathIndex];

        TowerPathProgress progress = GetPathProgress(pathIndex);

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

        upgradeProgress[pathIndex] = progress;

        return randomUpgrade;
    }

    public void PurchaseUpgrade(int pathIndex)
    {
        if (!InstanceFinder.IsServerStarted)
        {
            return;
        }

        TowerUpgradePathSO path = towerSO.UpgradePaths[pathIndex];

        TowerUpgradeSO upgrade = GetNextUpgrade(pathIndex);

        if (upgrade == null)
        {
            return;
        }

        if (BaseResourceController.Instance.RemoveResources(upgrade.RequiredResources))
        {
            //Let the upgrade apply itself.
            upgrade.GrantUpgrade(this);

            TowerPathProgress progress = GetPathProgress(pathIndex);

            progress.upgradeCount++;
            progress.pendingUpgradeID = -1;

            if (upgrade == path.MilestoneUpgrade)
            {
                progress.milestoneUpgradeCount++;
            }

            upgradeProgress[pathIndex] = progress;

            Debug.Log($"Upgrade {upgrade.UpgradeName} successfully purchased");
            path.GetRandomUpgrade();
        }
    }

    private TowerPathProgress GetPathProgress(int pathIndex)
    {
        if (!upgradeProgress.TryGetValue(pathIndex, out TowerPathProgress progress))
        {
            progress = new TowerPathProgress
            {
                upgradeCount = 0,
                pendingUpgradeID = -1
            };

            upgradeProgress.Add(pathIndex, progress);
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

    public void AddProjectileUpgrade(Tower tower, int amount)
    {
        if (!InstanceFinder.IsServerStarted)
        {
            return;
        }

        localUpgrades.projectileCountAdd += amount;

        OnUpgradesChanged();
    }

    /// <summary>
    /// Either creates or gets the upgrades for a specific tower type
    /// </summary>
    /// <param name="towerSO"></param>
    /// <returns></returns>
    public TowerUpgradesDC GetOrCreateLocalUpgrades(TowerSO towerSO)
    {
        return localUpgrades;
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
