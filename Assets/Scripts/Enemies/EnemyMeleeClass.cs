using UnityEngine;

public class EnemyMeleeClass : Enemy
{
    //References
    private WeaponHitbox weaponHitbox;
    private Animator enemyAnimator;

    //Stats
    public float EnemyDamage => enemyDamage;

    //Attack state
    private bool shouldAttack;
    private bool isAttacking = false;

    //Attack timer
    private float attackCooldown;

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the enemy melee class
    /// </summary>
    public void Initialize()
    {
        //Initialise vital components
        enemyAnimator = GetComponentInChildren<Animator>();
        weaponHitbox = GetComponentInChildren<WeaponHitbox>();

        //Initialize attack hitbox
        if (weaponHitbox != null)
        {
            weaponHitbox.Initialise(this);
        }
        else
        {
            Debug.LogWarning("EnemyMeleeClass: WeaponHitbox not found!", this);
        }

        // Reset attack state
        shouldAttack = false;
        isAttacking = false;
        attackCooldown = 0f;
    }

    private void Update()
    {
        AttackCooldown();
    }

    /// <summary>
    /// Updates the cooldown of enemy attacks
    /// </summary>
    private void AttackCooldown()
    {
        if (!IsServerStarted)
        {
            return;
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0f)
            {
                attackCooldown = 0f;

                Debug.Log("Attack cooldown finished");
            }
        }

        TryAttack();
    }

    /// <summary>
    /// Attempt attack when the brain tells it to
    /// </summary>
    public void StartAttacking()
    {
        Debug.Log("StartAttacking() called");

        if (!IsServerStarted)
        {
            Debug.Log("StartAttacking() stopped: not server");
            return;
        }

        shouldAttack = true;

        Debug.Log("shouldAttack set to TRUE");

        TryAttack();
    }

    /// <summary>
    /// Stops attacking when brain tells it to
    /// </summary>
    public void StopAttacking()
    {
        shouldAttack = false;
    }

    /// <summary>
    /// Checks if attacking is possible
    /// </summary>
    private void TryAttack()
    {
        if (!shouldAttack)
        {
            return;
        }

        if (isAttacking)
        {
            return;
        }

        if (attackCooldown > 0f)
        {
            return;
        }
        Debug.Log("TryAttack success");

        StartAttack();
    }

    /// <summary>
    /// Start attack animation if attacking is possible
    /// </summary>
    public void StartAttack()
    {
        isAttacking = true;

        if (enemyAttackRate > 0f)
        {
            attackCooldown = 1f / enemyAttackRate;
        }
        else
        {
            attackCooldown = 0f;
        }

        enemyAnimator.SetBool("isAttacking", true);
    }

    
    /// <summary>
    /// Reset attack
    /// </summary>
    public void EndAttack()
    {
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
    }
}
