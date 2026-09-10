using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VacuumProjectile : BaseProjectile
{
    private float AOE = 3f;

    protected override void HandleTriggerLogic(Collider other)
    {
        base.HandleTriggerLogic(other);

        List<Enemy> enemiesHit = GetHitEnemies();

        foreach (Enemy enemy in enemiesHit)
        {
            //Get the direction to the pull point
            Vector3 dir = (transform.position - enemy.transform.position).normalized;

            enemy.GetComponent<EnemyMovement>().PullTowards(dir, 2f, 0.5f);
        }
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
