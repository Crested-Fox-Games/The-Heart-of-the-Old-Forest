using UnityEngine;

public class EnemeyMeleeClass : Enemy
{
    public float EnemyDamage => enemySO.EnemyDamage;

    private WeaponHitbox weaponHitbox;


    ITargetable targetable;


    public void CallStartAttack()
    {
        weaponHitbox = GetComponent<WeaponHitbox>();

        weaponHitbox.StartAttack();
    }

    public void CallEndAttack()
    {
        weaponHitbox = GetComponent<WeaponHitbox>();

        weaponHitbox.EndAttack();
    }

}
