using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy/EnemySO")]
public class EnemySO : ScriptableObject
{
    [SerializeField]
    private string enemyName, enemyDescription;
    [SerializeField]
    private float enemyHealth, enemySpeed, enemyDamage, enemyAttackRate, enemyRange, enemySpawnWeight;
    [SerializeField]
    private GameObject projectile, enemyPrefab;

    /// <summary>
    /// The name of the enemy
    /// </summary>
    public string EnemyName => enemyName;

    /// <summary>
    /// The description for the enemy
    /// </summary>
    public string EnemyDescription => enemyDescription;

    /// <summary>
    /// The max health for the enemy
    /// </summary>
    public float EnemyHealth => enemyHealth;

    /// <summary>
    /// The speed the enemy moves at
    /// </summary>
    public float EnemySpeed => enemySpeed;

    /// <summary>
    /// The damage the enemy deals
    /// </summary>
    public float EnemyDamage => enemyDamage;

    /// <summary>
    /// The rate at which the enemy can attack
    /// </summary>
    public float EnemyAttackRate => enemyAttackRate;

    /// <summary>
    /// The range that the enemy can hit from (Only used in ranged enemies)
    /// </summary>
    public float EnemyRange => enemyRange;

    /// <summary>
    /// The weight used for determining how much the enemy spawn costs
    /// </summary>
    public float EnemySpawnWeight => enemySpawnWeight;

    /// <summary>
    /// The projectile an enemy will fire when it attacks (Only ranged and maybe magic?)
    /// </summary>
    public GameObject Projectile => projectile;

    /// <summary>
    /// The prefab object for the enemy
    /// </summary>
    public GameObject EnemyPrefab => enemyPrefab;
}
