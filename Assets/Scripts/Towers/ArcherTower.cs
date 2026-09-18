using GameKit.Dependencies.Utilities.ObjectPooling.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherTower : Tower
{
    private int volleyNumber = 0;

    private void OnTriggerEnter(Collider collision)
    {
        if(!IsServerStarted)
        { return; }

        Enemy enemy = collision.GetComponentInParent<Enemy>();

        //Null check
        if (enemy == null)
            return;

        Debug.Log(
       $"TRIGGER ENTER - Collider: {collision.name}, " +
       $"Enemy: {enemy.name}, " +
       $"Enemy ID: {enemy.gameObject.GetInstanceID()}"
   );

        AddEnemyToTargets(enemy);

        //set the current target if none set already
        if(targetEnemy == null)
        {
            targetEnemy = enemy.gameObject;
        }

        Debug.Log(
        $"Starting attack on {targetEnemy.name}. " +
        $"Targets count: {targets.Count}"
    );

        StartAttack();
    }

    private void OnTriggerExit(Collider collision)
    {
        if(!IsServerStarted) 
        { return; }

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
                //targets.RemoveAt(0);
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
        Debug.Log("AttackLoop STARTED");
        //Checks to ensure there is a target to hit
        while (targetEnemy != null)
        {
            if (stunned) //TODO: add some sort of stun time.
            {
                yield return new WaitForSeconds(3f);
                continue;
            }

            volleyNumber++;

            int currentVolley = volleyNumber;

            List<GameObject> splitFireTargets = GetSplitFireTargets();
            //int projectileCount = GetProjectileCount();

            Debug.Log(
           $"=== VOLLEY {currentVolley} === " +
           $"Target count: {splitFireTargets.Count}"
            );

            for (int i = 0; i < splitFireTargets.Count; i++)
            {
                GameObject target = splitFireTargets[i];

                Debug.Log(
                    $"Volley {currentVolley} | " +
                    $"Projectile {i + 1}/{splitFireTargets.Count} | " +
                    $"Target: {(target != null ? target.name : "NULL")}"
                );

                if (target == null)
                    continue;

                GameObject proj = Instantiate(
                    projectile,
                    transform.position,
                    transform.rotation
                );

                proj.GetComponent<NormalProjectile>().InitializeProjectile(
                    target.transform.position,
                    GetDamage()
                );

                Spawn(proj);
            }


            //Activate cooldown
            yield return new WaitForSeconds(GetFireRate());

            targets.RemoveAll(target => target == null);

            //Checks if the target enemy has been killed, and if so, adds a new target from the list
            if (targetEnemy == null && targets.Count > 0)
            {
                while(targets.Count > 0)
                {
                    targetEnemy = targets[0];
                    //targets.RemoveAt(0);

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
        GameObject enemyObj = enemy.gameObject;

        Debug.Log(
            $"AddEnemyToTargets called: {enemyObj.name} " +
            $"ID: {enemyObj.GetInstanceID()} " +
            $"Targets count: {targets.Count} " +
            $"Current target: {(targetEnemy != null ? targetEnemy.name : "NULL")}"
        );

        // Avoid duplication
        if (targets.Contains(enemyObj))
        {
            Debug.Log("Enemy already exists in targets.");
            return;
        }

        if (targetEnemy == enemyObj)
        {
            Debug.Log("Enemy is already the current target.");
            return;
        }

        enemy.onEnemyKilled += RemoveEnemyFromTargets;

        if (targetEnemy != null)
        {
            targets.Add(enemyObj);

            Debug.Log($"Added {enemyObj.name} to targets. New count: {targets.Count}");
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

    /// <summary>
    /// Get list of enemies as targets for split fire ability
    /// </summary>
    /// <returns></returns>
    private List<GameObject> GetSplitFireTargets()
    {
        List<GameObject> splitFireTargets = new List<GameObject>();

        if (targetEnemy != null)
        {
            splitFireTargets.Add(targetEnemy);
        }

        foreach (GameObject target in targets)
        {
            if (target == null)
            {
                continue;
            }

            if (splitFireTargets.Contains(target))
            {
                continue;
            }

            splitFireTargets.Add(target);

            if (splitFireTargets.Count >= GetProjectileCount())
            {
                break;
            }
        }
        
        return splitFireTargets;
        }
    }
