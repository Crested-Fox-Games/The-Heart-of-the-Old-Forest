using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using UnityEngine;

public class NormalProjectile : BaseProjectile
{
    /// <summary>
    /// The target position the projectile is moving towards
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// The direction the projectile is moving in
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Initializes the projectiles initial values
    /// </summary>
    /// <param name="target"></param>
    /// <param name="projectileDamage"></param>
    /// <param name="tower"></param>
    public override void InitializeProjectile(Vector3 target, float projectileDamage)
    {
        targetPosition = target;
        projDamage = projectileDamage;

        direction = (targetPosition - transform.position).normalized;
        StartCoroutine(MoveToTarget());
    }

    /// <summary>
    /// Moves the projectile towards the target position over time until it either hits the target or times out
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToTarget()
    {
        float timer = 0;

        while (timer < projectileMaxTime)
        {
            timer += Time.deltaTime;

            //Move the projectile towards the enemy
            transform.position += direction * projSpeed * Time.deltaTime;

            yield return null;
        }

        HandleProjectileFinished();
    }


    protected override void HandleProjectileEnemyHit(Enemy enemy)
    {
        //Deal damage
        enemy.TakeDamage(projDamage);
        HandleProjectileFinished();
    }

    protected override void HandleProjectileBlightNodeHit(BlightNode blightNode)
    {
        blightNode.TakeDamage(projDamage);
        HandleProjectileFinished();
    }

    
}
