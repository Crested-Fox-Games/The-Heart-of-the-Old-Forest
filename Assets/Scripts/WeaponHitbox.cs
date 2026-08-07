using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : NetworkBehaviour
{
    //References
    [SerializeField] private Collider hitboxCollider;

    //HashSet
    private HashSet<StructureHealth> hitTargets = new HashSet<StructureHealth>();
    private EnemyMeleeClass owner;

    //Attack bool
    private bool attackActive;

    
    /// <summary>
    /// Initialise owner of the hitbox, see EnemyMeleeClass for implementation
    /// </summary>
    /// <param name="melee"></param>
    public void Initialise(EnemyMeleeClass melee)
    {
        owner = melee;
    }

    /// <summary>
    /// Starts the attack, clears HashSet targets ready for next attack, enables hitboxCollider during attack animation
    /// </summary>
    public void StartAttack()
    {
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    /// <summary>
    /// Ends the attack by disabling hitboxCollider during attack animation
    /// </summary>
    public void EndAttack()
    {
        attackActive = false;
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Assigns damage to objects that contain the StructureHealth script while attack is active
    /// </summary>
    /// <param name="other"></param>
    private void AttackTrigger(Collider other)
    {
        if (!IsServerStarted)
            return;

        if (!attackActive)
            return;

       
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

    private void OnTriggerEnter(Collider other)
    {
        AttackTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        AttackTrigger(other);
    }
}
