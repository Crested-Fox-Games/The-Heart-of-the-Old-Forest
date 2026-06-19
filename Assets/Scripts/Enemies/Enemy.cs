using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

//TODO: make this class an abstract parent when implementing enemy types
public class Enemy : NetworkBehaviour
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

    public readonly SyncVar<float> currentHealth = new();

    private GameObject heartCrystal;

    public GameObject HeartCrystal => heartCrystal;

    public event Action onEnemyKilled;

    public override void OnStartServer()
    {
        heartCrystal = FindFirstObjectByType<HeartCrystal>()?.gameObject;

        enemyBrain = GetComponent<EnemyBrain>();
        enemyMovement = GetComponent<EnemyMovement>();

        InitializeValues();

        currentHealth.Value = enemyHealth;
    }

    /// <summary>
    /// Sets the initial values of the enemy based on the SO
    /// </summary>
    public void InitializeValues()
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

        //Starts the initialization for the enemy scripts
        enemyMovement.Initialize();
        enemyBrain.Initialize(HeartCrystal);
    }

    //TODO: Probably add an enum for proj types to easily trigger effects
    public void TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return;

        Debug.Log($"{damage} damage dealt");

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Death();
        }
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
