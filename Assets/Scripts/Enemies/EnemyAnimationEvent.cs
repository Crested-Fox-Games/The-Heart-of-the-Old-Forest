using UnityEngine;
using FishNet.Object;

public class EnemyAnimationEvent : NetworkBehaviour
{
    //Reference to weaponHitbox script
    [SerializeField] private WeaponHitbox weaponHitbox;

    private EnemyMeleeClass enemyMeleeClass;

    private void Awake()
    {
        enemyMeleeClass = GetComponentInParent<EnemyMeleeClass>();
    }

    /// <summary>
    /// Call weaponHitbox StartAttack function on animation event
    /// </summary>
    public void CallStartAttack()
    {
        weaponHitbox.StartAttack();
    }

    /// <summary>
    /// Call weaponHitbox EndAttack function on animation event
    /// </summary>
    public void CallEndAttack()
    {
        Debug.Log("CALL END ATTACK EVENT FIRED");

        weaponHitbox.EndAttack();

        enemyMeleeClass.EndAttack();
    }
}
