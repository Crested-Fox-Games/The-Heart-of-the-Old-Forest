using GameKit.Dependencies.Utilities.ObjectPooling.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherTower : Tower
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if (targetEnemy == null) //If we're not targeting an enemy, start targeting them.
            {
                targetEnemy = collision.gameObject;
                Debug.Log("Setting target");
            }
            else //If we are already targeting something, add it to the list of possible targets.
            {
                targets.Add(collision.gameObject);
            }
            Debug.Log($"Starting attack on {targetEnemy}");
            StartAttack();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if (collision.gameObject != targetEnemy)
            {
                targets.Remove(collision.gameObject);
                return;
            }

            if (targetEnemy != null) //If we are targetting something
            {
                if (targets.Count <= 0) //If theres no other targets in range.
                {
                    targetEnemy = null;
                }
                else //Select a new target
                {
                    //TODO: Possibly add a way to change targetting system, e.g fist, last, strongest, weakest
                    targetEnemy = targets[0];
                    targets.RemoveAt(0);
                }
            }
        }
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
}
