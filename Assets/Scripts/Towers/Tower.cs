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

    private readonly SyncDictionary<TowerStats, float> towerAdditiveUpgrades = new();

    private readonly SyncDictionary<TowerStats, float> towerMultiplicativeUpgrades = new();

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

        IntitializeUpgradeDictionaries();

        currentHealth.Value = towerMaxHealth;

        towerAdditiveUpgrades.OnChange += OnUpgradesChanged;
        towerMultiplicativeUpgrades.OnChange += OnUpgradesChanged;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        towerAdditiveUpgrades.OnChange -= OnUpgradesChanged;
        towerMultiplicativeUpgrades.OnChange -= OnUpgradesChanged;
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
    }

    /// <summary>
    /// Initializes the towers upgrade dictionaries
    /// </summary>
    private void IntitializeUpgradeDictionaries()
    {
        towerAdditiveUpgrades[TowerStats.Attack] = 0f;
        towerAdditiveUpgrades[TowerStats.FireRate] = 0f;
        towerAdditiveUpgrades[TowerStats.Health] = 0f;
        towerAdditiveUpgrades[TowerStats.Range] = 0f;

        towerMultiplicativeUpgrades[TowerStats.Attack] = 1f;
        towerMultiplicativeUpgrades[TowerStats.FireRate] = 1f;
        towerMultiplicativeUpgrades[TowerStats.Health] = 1f;
        towerMultiplicativeUpgrades[TowerStats.Range] = 1f;
    }

    public void TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value < 0)
        {
            //TODO: Need to create some sort of broken form 
            //TODO: Might want a way to repair it when its damaged

            //Tells the tower placement that this tower has died
            TempTowerPlacement towerPlacement = GetComponentInParent<TempTowerPlacement>();

            //TODO: Change this when above TODO's are done
            towerPlacement.TowerDestroyed();
        }
    }

    /// <summary>
    /// Adds the upgrades to the dictionary
    /// </summary>
    /// <param name="towerStat"></param>
    /// <param name="upgradeType"></param>
    /// <param name="amount"></param>
    public void AddUpgrade(TowerStats towerStat, UpgradeType upgradeType, float amount)
    {
        if(upgradeType == UpgradeType.Addition)
        {
            towerAdditiveUpgrades[towerStat] += amount;
        }    
        else
        {
            towerMultiplicativeUpgrades[towerStat] += amount;
        }
    }

    private void OnUpgradesChanged(SyncDictionaryOperation op, TowerStats key, float value, bool asServer)
    {
        switch(key)
        {
            case TowerStats.Range:
                //Updates the range of the tower
                towerRangeCollider.radius = GetRange();
                break;

            case TowerStats.Health:

                //TODO: Factor in losing max health(if possible)
                //Updates the health of the tower
                float newMaxHealth = GetHealth();

                float difference = newMaxHealth - towerMaxHealth;

                towerMaxHealth = newMaxHealth;
                currentHealth.Value += difference;
                break;
        }
    }

    protected float GetDamage()
    {
        return (towerDamage + towerAdditiveUpgrades[TowerStats.Attack]) * towerMultiplicativeUpgrades[TowerStats.Attack];
    }

    protected float GetFireRate()
    {
        return (attackCooldown + towerAdditiveUpgrades[TowerStats.FireRate]) * towerMultiplicativeUpgrades[TowerStats.FireRate];
    }

    protected float GetRange()
    {
        return (attackRange + towerAdditiveUpgrades[TowerStats.Range]) * towerMultiplicativeUpgrades[TowerStats.Range];
    }

    protected float GetHealth()
    {
        return (towerDamage + towerAdditiveUpgrades[TowerStats.Health]) * towerMultiplicativeUpgrades[TowerStats.Health];
    }
}
