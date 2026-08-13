using UnityEngine;

public class EnemyMeleeClass : Enemy
{
    //References
    private WeaponHitbox weaponHitbox;
    public float EnemyDamage => enemyDamage;
    
    private ITargetable currentTarget;

    private Animator enemyAnimator;
    private bool isAttacking = false;
    private void Awake()
    {
        //Initialise vital components
        enemyAnimator = GetComponentInChildren<Animator>();

        weaponHitbox = GetComponentInChildren<WeaponHitbox>();

        weaponHitbox.Initialise(this);
    }

    private void Start()
    {
        if (!IsServerStarted)
            return;

        isAttacking = true;
        enemyAnimator.Play("BadgerAttack");
    }
}
