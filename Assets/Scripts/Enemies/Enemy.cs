using Mono.Cecil;
using System;
using UnityEngine;

//TODO: make this class an abstract parent when implementing enemy types
public class Enemy : MonoBehaviour
{
    #region SO Fields
    [SerializeField]
    private EnemySO enemySO;

    private string enemyName, enemyDescription;
    private float enemyHealth, enemySpeed, enemyDamage, enemyAttackRate, enemySpawnWeight;

    public EnemySO EnemySO => enemySO;

    #endregion

    //References
    private EnemyBrain enemyBrain;
    private EnemyMovement enemyMovement;

    private float currentHealth;

    private GameObject heartCrystal;

    public GameObject HeartCrystal => heartCrystal;

    public event Action onEnemyKilled;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        enemyMovement = GetComponent<EnemyMovement>();

        currentHealth = enemyHealth;
    }

    /// <summary>
    /// Sets the initial values of the enemy based on the SO
    /// </summary>
    public void InitializeValues(GameObject HeartCrystal)
    {
        //Strings
        enemyName = enemySO.EnemyName;
        enemyDescription = enemySO.EnemyDescription;

        //Floats
        enemyHealth = enemySO.EnemyHealth;
        enemySpeed = enemySO.EnemySpeed;
        enemyDamage = enemySO.EnemyDamage;
        enemyAttackRate = enemySO.EnemyAttackRate;
        enemySpawnWeight = enemySO.EnemySpawnWeight;

        SetHeartCrystal(HeartCrystal);

        //Starts the initialization for the enemy scripts
        enemyMovement.Initialize();
        enemyBrain.Initialize(HeartCrystal);
    }

    //TODO: Probably add an enum for proj types to easily trigger effects
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} damage dealt");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void SetHeartCrystal(GameObject heartCrystal)
    {
        this.heartCrystal = heartCrystal;
    }

    private void Death()
    {
        onEnemyKilled?.Invoke();

        //TODO: add death functionality
    }

    /// <summary>
    /// This will be run by the game when day starts so that enemies will be removed without giving rewards
    /// </summary>
    public void GameDeath()
    {
        //TODO: Check with design team if this is needed
    }
}
