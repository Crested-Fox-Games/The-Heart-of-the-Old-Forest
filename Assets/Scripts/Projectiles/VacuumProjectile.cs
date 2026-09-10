using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VacuumProjectile : BaseProjectile
{
    private float AOE = 3f;

    [SerializeField]
    private LayerMask mask;

    /// <summary>
    /// The target position the projectile is moving towards
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// The direction the projectile is moving in
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Bool to ensure no double ups
    /// </summary>
    private bool hasHit = false;

    /// <summary>
    /// Initializes the projectiles initial values
    /// </summary>
    /// <param name="target"></param>
    /// <param name="projectileDamage"></param>
    /// <param name="tower"></param>
    public override void InitializeProjectile(Vector3 target, float projectileDamage)
    {
        Debug.Log("Initializing vacuum proj");
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
        Debug.Log("Vacuum proj should be moving");
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

    protected override void HandleTriggerLogic(Collider other)
    {
        //base.HandleTriggerLogic(other);

        if (other.isTrigger)
            return;

        //Checks if the layer is on a different one to the ones we selected in the mask
        if(((1 << other.gameObject.layer) & mask) == 0)
        {
            return;
        }

        if (hasHit)
            return;

        hasHit = true;

        Debug.Log($"Vacuum Proj detected that it has hit {other.gameObject.name} which it considers valid");

        List<Enemy> enemiesHit = GetHitEnemies();

        foreach (Enemy enemy in enemiesHit)
        {
            Debug.Log($"Enemy {enemy.name} has been hit, there are {enemiesHit.Count} total hits");
            //Get the direction to the pull point
            Vector3 dir = (transform.position - enemy.transform.position).normalized;

            enemy.GetComponent<EnemyMovement>().PullTowards(dir, 2f, 0.5f);
        }

        HandleProjectileFinished();
    }

    //Unsure if this is meant to deal damage or not
    protected override void HandleProjectileBlightNodeHit(BlightNode blightNode)
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleProjectileEnemyHit(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }

    private List<Enemy> GetHitEnemies()
    {
        return Physics.OverlapSphere(transform.position, AOE)
            .Select(collider => collider.GetComponentInParent<Enemy>())
            .Where(go => (go != null))
            .Distinct()
            .OrderBy(go => (go.transform.position - transform.position).sqrMagnitude)
            .ToList();
    }
}
