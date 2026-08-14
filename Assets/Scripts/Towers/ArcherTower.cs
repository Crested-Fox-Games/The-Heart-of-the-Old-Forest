using GameKit.Dependencies.Utilities.ObjectPooling.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherTower : Tower
{
    private void OnTriggerEnter(Collider collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();

        //Null check
        if (enemy == null)
            return;

        AddEnemyToTargets(enemy);

        //set the current target if none set already
        if(targetEnemy == null)
        {
            targetEnemy = enemy.gameObject;
        }
 
        Debug.Log($"Starting attack on {targetEnemy}");
        StartAttack();
    }

    private void OnTriggerExit(Collider collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        //Remove the enemy from the list if its not the current enemy
        if(collision.gameObject != targetEnemy)
        {
            RemoveEnemyFromTargets(enemy);
            return;
        }

        if (targetEnemy != null)
        {
            if(targets.Count <= 0)
            {
                targetEnemy = null;
            }
            else
            {
                //Update the target enemy to the first one in the list
                //TODO: This is one of the places we need to implement tower targetting
                targetEnemy = targets[0];
                targets.RemoveAt(0);
            }
        }

        enemy.onEnemyKilled -= RemoveEnemyFromTargets;
    }

    private void StartAttack()
    {
        if (attackCoroutine == null) //If the attack coroutine isnt running, start it.
        {
            attackCoroutine = StartCoroutine(AttackLoop());
        }
    }

    private IEnumerator AttackLoop()
    {
        //Checks to ensure there is a target to hit
        while (targetEnemy != null)
        {
            if (stunned) //TODO: add some sort of stun time.
            {
                yield return new WaitForSeconds(3f);
                continue;
            }

            //Spawn projectile with stats
            //For optimization turn projs off and on instead of destroying
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);

            //Initialize the projectile
            proj.GetComponent<Projectile>().InitializeProjectile(targetEnemy.transform.position, towerDamage);

            Spawn(proj);

            //Activate cooldown
            yield return new WaitForSeconds(attackCooldown);


            //Checks if the target enemy has been killed, and if so, adds a new target from the list
            if (targetEnemy == null && targets.Count > 0)
            {
                while(targets.Count > 0)
                {
                    targetEnemy = targets[0];
                    targets.RemoveAt(0);

                    if (targetEnemy != null)
                        break;
                }
                
            }
            
            if (targetEnemy == null)
            {
                //Ends the ienumerator as there are no targets to hit
                break;
            }
        }

        attackCoroutine = null;
    }

    /// <summary>
    /// Adds the enemy to the target list and subscribes to its death event
    /// </summary>
    /// <param name="enemy"></param>
    private void AddEnemyToTargets(Enemy enemy)
    {
        //Avoid duplication
        if (targets.Contains(enemy.gameObject) || targetEnemy == enemy.gameObject)
            return;

        //Subscribes to the event so that when it dies it will be removed
        enemy.onEnemyKilled += RemoveEnemyFromTargets;

        if(targetEnemy != null)
        {
            targets.Add(enemy.gameObject);
        }
    }

    /// <summary>
    /// Removes the enemy from the target list and unsubscribes from its death event
    /// </summary>
    /// <param name="enemy"></param>
    private void RemoveEnemyFromTargets(Enemy enemy)
    {
        enemy.onEnemyKilled -= RemoveEnemyFromTargets;

        GameObject enemyObj = enemy.gameObject;

        targets.Remove(enemyObj);

        if(targetEnemy == enemyObj)
        {
            targetEnemy = null;
        }
    }
}
