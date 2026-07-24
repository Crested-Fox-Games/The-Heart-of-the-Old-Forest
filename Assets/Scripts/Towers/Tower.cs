using FishNet.Object;
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

    protected Coroutine attackCoroutine;

    private void Start()
    {
        InitializeValues();

        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = attackRange;
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

    
}
