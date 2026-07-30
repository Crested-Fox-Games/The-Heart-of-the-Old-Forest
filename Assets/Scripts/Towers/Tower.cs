using FishNet.Object;
using FishNet.Object.Synchronizing;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : NetworkBehaviour
{
    #region SO Fields
    [SerializeField]
    protected TowerSO towerSO;

    protected string towerName, towerDescription;

    protected float attackRange, towerDamage, towerHealth, attackCooldown;

    protected GameObject projectile, displayObject;

    #endregion

    protected GameObject targetEnemy;

    protected List<GameObject> targets = new List<GameObject>();

    protected bool stunned = false;

    /// <summary>
    /// The current health of the tower
    /// </summary>
    protected readonly SyncVar<float> currentHealth = new();

    protected Coroutine attackCoroutine;

    private void Start()
    {
        InitializeValues();

        //Creates the sphere around the tower that they can attack in
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = attackRange;
        col.isTrigger = true;

        currentHealth.Value = towerHealth;
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
        towerHealth = towerSO.TowerHealth;
        attackCooldown = towerSO.AttackCooldown;

        //GameObjects
        projectile = towerSO.Projectile;
        displayObject = towerSO.DisplayObject;
    }

    public void TakeDamage(float damage)
    {
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
    
}
