using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

//TODO: make this class an abstract parent when implementing enemy types
public class Enemy : NetworkBehaviour
{
    #region SO Fields
    [SerializeField]
    private EnemySO enemySO;

    private string enemyName, enemyDescription;
    protected float enemyHealth, enemySpeed, enemyDamage, enemyAttackRate, enemySpawnWeight;

    public EnemySO EnemySO => enemySO;

    #endregion

    //References
    private EnemyBrain enemyBrain;
    private EnemyMovement enemyMovement;
    private EnemySpawner enemySpawner;

    /// <summary>
    /// The current health of the enemy, the syncvar allows this variable to be updated across
    /// the network whenever it is changed so that all of the clients have the same value
    /// </summary>
    public readonly SyncVar<float> currentHealth = new();

    private GameObject heartCrystal;

    public GameObject HeartCrystal => heartCrystal;

    public event Action onEnemyKilled;

    public override void OnStartServer()
    {
        heartCrystal = FindFirstObjectByType<HeartCrystal>()?.gameObject;
        enemySpawner = FindFirstObjectByType<EnemySpawner>();

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

        //TODO: add any rewards for enemies being killed

        ServerManager.Despawn(gameObject);
        
    }

    /// <summary>
    /// This will be run by the game when day starts so that enemies will be removed without giving rewards
    /// </summary>
    public void GameDeath()
    {
        StartCoroutine(SlowDeath());
    }

    private IEnumerator SlowDeath()
    {
        float startEnemyHealth = currentHealth.Value;
        while (currentHealth.Value > 0f)
        {
            //Reduces the enemies health by 10% of what it was at the start of day.
            currentHealth.Value -= startEnemyHealth / 10;

            //Waits 1 second to do more damage
            yield return new WaitForSeconds(1f);
        }

        onEnemyKilled?.Invoke();

        //Handle death here, no rewards for this death if we're doing rewards for killing enemies.
        ServerManager.Despawn(gameObject);
    }

}
