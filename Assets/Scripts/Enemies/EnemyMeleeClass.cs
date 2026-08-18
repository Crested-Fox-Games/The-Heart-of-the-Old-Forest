using UnityEngine;

public class EnemyMeleeClass : Enemy
{
    //References
    private WeaponHitbox weaponHitbox;
    private Animator enemyAnimator;
    public float EnemyDamage => enemyDamage;

    //Attack state
    private bool shouldAttack;
    private bool isAttacking = false;

    //Attack timer
    private float attackCooldown;


    private void Awake()
    {
        //Initialise vital components
        enemyAnimator = GetComponentInChildren<Animator>();

        weaponHitbox = GetComponentInChildren<WeaponHitbox>();

        weaponHitbox.Initialise(this);
    }

    private void Update()
    {
        AttackCooldown();
    }

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

    public void StopAttacking()
    {
        shouldAttack = false;
    }

    private void TryAttack()
    {
        if (!shouldAttack)
        {
            //Debug.Log("TryAttack blocked: shouldAttack is false");
            return;
        }

        if (isAttacking)
        {
            //Debug.Log("TryAttack blocked: isAttacking is true");
            return;
        }

        if (attackCooldown > 0f)
        {
            return;
        }
        Debug.Log("TryAttack success");

        StartAttack();
    }

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

        enemyAnimator.Play("BadgerAttack");
    }

    

    public void EndAttack()
    {
        isAttacking = false;

        Debug.Log("EnemyMeleeClass EndAttack() called. isAttacking = FALSE");
    }
}
