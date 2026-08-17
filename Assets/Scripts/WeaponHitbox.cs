using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : NetworkBehaviour
{
    //References
    //Hitbox on enemy weapon
    [SerializeField] private Collider hitboxCollider;

    //HashSet 
    //Stores unique targets that are hit, similar to a list but can't add the same object twice
    private HashSet<ITargetable> hitTargets = new HashSet<ITargetable>();
    //Used to access damage float
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
    /// Enables attack during attack animation
    /// </summary>
    public void StartAttack()
    {;
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    /// <summary>
    /// Disables attack during attack animation
    /// </summary>
    public void EndAttack()
    {
        attackActive = false;
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Assigns damage to objects that implement ITargetable while the attack is active
    /// </summary>
    /// <param name="other"></param>
    private void AttackTrigger(Collider other)
    {
        if (!IsServerStarted)
            return;

        if (!attackActive)
            return;

       
        //damage logic
        ITargetable target = other.GetComponent<ITargetable>();

        if (target == null)
        {
            return;
        }

        //hitTargets.Add returns a bool, if the object is already stored in the hashset, returns false and exits function
        if (!hitTargets.Add(target))
        {
            return;
        }

        //Evaluate new enemies when current target is destroyed
        if (!target.TakeDamage(owner.EnemyDamage))
        {
            EnemyBrain brain = GetComponentInParent<EnemyBrain>();

            brain.ReevaluateTargets();
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        AttackTrigger(other);
    }

    /// <summary>
    /// Evaluates new targets for enemies whenever a triggered attack ends
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        EnemyBrain brain = GetComponentInParent<EnemyBrain>();

        brain.ReevaluateTargets();
    }
}
