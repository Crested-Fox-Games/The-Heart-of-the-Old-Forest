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

    /// <summary>
    /// The current health of the tower
    /// </summary>
    protected readonly SyncVar<float> currentHealth = new();

    protected Coroutine attackCoroutine;

    TowerManager towerManager;

    private SphereCollider towerRangeCollider;

    private void Awake()
    {
        InitializeValues();

        //Creates the sphere around the tower that they can attack in
        towerRangeCollider = gameObject.AddComponent<SphereCollider>();
        towerRangeCollider.radius = GetRange();
        towerRangeCollider.isTrigger = true;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth.Value = towerMaxHealth;

        towerManager = FindFirstObjectByType<TowerManager>();
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

        if(towerManager.GlobalTowerUpgrades.ContainsKey(towerSO))
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

        return (towerDamage + globalUpgrades.attackAdd) * globalUpgrades.attackMult;
    }

    protected float GetFireRate()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);

        return (attackCooldown + globalUpgrades.fireRateAdd) * globalUpgrades.fireRateMult;
    }

    protected float GetRange()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);

        return (attackRange + globalUpgrades.rangeAdd) * globalUpgrades.rangeMult;
    }

    protected float GetHealth()
    {
        GlobalTowerUpgradesDC globalUpgrades = towerManager.GetOrCreateGlobalUpgrades(towerSO);

        return (towerMaxHealth + globalUpgrades.healthAdd) * globalUpgrades.healthMult;
    }
}
