using UnityEngine;

[CreateAssetMenu(fileName = "TowerSO", menuName = "Scriptable Objects/TowerSO")]
public class TowerSO : ScriptableObject
{
    [SerializeField]
    private string towerName, towerDescription;

    [SerializeField]
    private float attackRange, towerHealth, towerDamage, attackCooldown;

    [SerializeField]
    private GameObject projectile, displayObject;

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
}
