using FishNet.Object;
using UnityEngine;

public abstract class BaseProjectile : NetworkBehaviour
{
    /// <summary>
    /// The speed at which the projectile moves towards its target
    /// </summary>
    [SerializeField]
    protected float projSpeed;

    /// <summary>
    /// The amount of damage the projectile will deal to the target
    /// </summary>
    protected float projDamage;

    /// <summary>
    /// The maximum time the projectile can exist before being destroyed
    /// </summary>
    protected float projectileMaxTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerLogic(other);
    }

    /// <summary>
    /// This allows us to override the logic for things like enemy projectiles that should hit players and 
    /// structures instead of enemies and blight nodes
    /// </summary>
    /// <param name="other"></param>
    protected virtual void HandleTriggerLogic(Collider other)
    {
        //Checks to see if the projectile hit an enemy or a blight node
        Enemy hitEnemy = other.GetComponentInParent<Enemy>();
        BlightNode hitBlightNode = other.GetComponentInParent<BlightNode>();

        //Tells the projectile to handle the hit based on what it hit
        if (hitEnemy != null)
        {
            HandleProjectileEnemyHit(hitEnemy);
        }
        else if (hitBlightNode != null)
        {
            HandleProjectileBlightNodeHit(hitBlightNode);
        }
    }

    /// <summary>
    /// Handles the logic for when we hit an enemy
    /// </summary>
    /// <param name="enemy"></param>
    protected abstract void HandleProjectileEnemyHit(Enemy enemy);

    /// <summary>
    /// Handles the logic for when we hit a blight node
    /// </summary>
    /// <param name="blightNode"></param>
    protected abstract void HandleProjectileBlightNodeHit(BlightNode blightNode);

    /// <summary>
    /// Handles what happens when the projectile hits its target or times out
    /// </summary>
    protected void HandleProjectileFinished()
    {
        ServerManager.Despawn(gameObject);
    }
}
