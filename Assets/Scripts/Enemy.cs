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

    public float EnemySpawnWeight => enemySpawnWeight;

    #endregion

    private float currentHealth;

    private GameObject heartCrystal;

    public event Action onEnemyKilled;

    private void Start()
    {
        InitializeValues();
        currentHealth = enemyHealth;
    }

    /// <summary>
    /// Sets the initial values of the enemy based on the SO
    /// </summary>
    private void InitializeValues()
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

    public void SetHeartCrystal(GameObject heartCrystal)
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
