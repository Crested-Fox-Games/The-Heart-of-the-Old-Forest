using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used instead of a dictionary in order to be able to fill it in while working in the inspector
/// </summary>
[System.Serializable]
public class ResourceCost
{
    public ResourceType resource;
    public int cost;
}

[CreateAssetMenu(fileName = "TowerSO", menuName = "Towers/TowerSO")]
public class TowerSO : ScriptableObject
{
    [SerializeField]
    private string towerName, towerDescription;

    [SerializeField]
    private float attackRange, towerHealth, towerDamage, attackCooldown;

    [SerializeField]
    private GameObject projectile, displayObject;

    [SerializeField]
    private List<ResourceCost> requiredResources;

    /// <summary>
    /// The name of the tower
    /// </summary>
    public string TowerName => towerName;

    /// <summary>
    /// The description of the tower
    /// </summary>
    public string TowerDescription => towerDescription;

    /// <summary>
    /// The range the tower can fire at enemies
    /// </summary>
    public float AttackRange => attackRange;

    /// <summary>
    /// The max health of the tower
    /// </summary>
    public float TowerHealth => towerHealth;

    /// <summary>
    /// The damage the tower deals
    /// </summary>
    public float TowerDamage => towerDamage;

    /// <summary>
    /// The cooldown between attacks
    /// </summary>
    public float AttackCooldown => attackCooldown;

    /// <summary>
    /// The projectile that is sent when the tower attacks
    /// </summary>
    public GameObject Projectile => projectile;

    /// <summary>
    /// For placing into world, shows an outline
    /// </summary>
    public GameObject DisplayObject => displayObject;

    /// <summary>
    /// A dictionary containing the required resources and the amounts required
    /// </summary>
    public List<ResourceCost> RequiredResources => requiredResources;
}
