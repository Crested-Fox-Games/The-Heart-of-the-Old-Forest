using UnityEngine;

public class EnemeyMeleeClass : Enemy
{
    ITargetable targetable;

    private void Target()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        targetable.TakeDamage(enemyDamage);
    }
}
