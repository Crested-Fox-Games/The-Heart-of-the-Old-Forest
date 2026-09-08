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
        Enemy hitEnemy = other.GetComponentInParent<Enemy>();
        BlightNode hitBlightNode = other.GetComponentInParent<BlightNode>();

        if(hitEnemy != null)
        {
            HandleProjectileEnemyHit(hitEnemy);
        }
        else if (hitBlightNode != null)
        {
            HandleProjectileBlightNodeHit(hitBlightNode);
        }
    }

    protected abstract void HandleProjectileEnemyHit(Enemy enemy);
    protected abstract void HandleProjectileBlightNodeHit(BlightNode blightNode);
}
