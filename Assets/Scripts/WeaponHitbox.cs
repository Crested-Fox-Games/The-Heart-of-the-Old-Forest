using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private Collider hitboxCollider;

    private EnemeyMeleeClass owner;

    private bool attackActive;

    private HashSet<StructureHealth> hitTargets = new HashSet<StructureHealth>();

    public void Initialise(EnemeyMeleeClass melee)
    {
        owner = melee;
    }

    public void StartAttack()
    {
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    public void EndAttack()
    {
        attackActive = false;
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackActive)
        {
            return;
        }
        else
        {
            //damage logic
            StructureHealth health = other.GetComponent<StructureHealth>();

            if (health == null)
            {
                return;
            }
            if (!hitTargets.Add(health))
            {
                return;
            }

            health.TakeDamage(owner.EnemyDamage);
        }
    }
}
