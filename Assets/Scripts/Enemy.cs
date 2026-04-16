using Mono.Cecil;
using UnityEngine;

//TODO: make this class an abstract parent when implementing enemy types
public class Enemy : MonoBehaviour
{
    #region SO Fields
    [SerializeField]
    private EnemySO enemySO;

    private string enemyName, enemyDescription;
    private float enemyHealth, enemySpeed, enemyDamage;

    #endregion

    private float currentHealth ;

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

    }

    //TODO: Probably add an enum for proj types to easily trigger effects
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} damage dealt");

        currentHealth -= damage;
    }
}
