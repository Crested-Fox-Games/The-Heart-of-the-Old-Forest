using UnityEngine;
using FishNet.Object;

public class EnemyAnimationEvent : NetworkBehaviour
{
    //Reference to weaponHitbox script
    [SerializeField] private WeaponHitbox weaponHitbox;

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
        weaponHitbox.EndAttack();
    }
}
