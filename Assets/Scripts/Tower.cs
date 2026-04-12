using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private TowerSO towerSO;

    private string towerName, towerDescription;

    private float attackRange, towerDamage, towerHealth, attackCooldown;

    private GameObject projectile;

    private GameObject displayObject;

    private GameObject targetEnemy;

    private List<GameObject> targets = new List<GameObject>();

    private bool stunned = false;

    private Coroutine attackCoroutine;

    private void Start()
    {
        InitializeValues();

        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = attackRange;
    }

    private void InitializeValues()
    {
        towerName = towerSO.TowerName;
        towerDescription = towerSO.TowerDescription;

        attackRange = towerSO.AttackRange;
        towerDamage = towerSO.TowerDamage;
        towerHealth = towerSO.TowerHealth;
        attackCooldown = towerSO.AttackCooldown;

        projectile = towerSO.Projectile;
        displayObject = towerSO.DisplayObject;
 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if(targetEnemy == null) //If we're not targeting an enemy, start targeting them.
            {
                targetEnemy = collision.gameObject;
                Debug.Log("Setting target");
            }
            else //If we are already targeting something, add it to the list of possible targets.
            {
                targets.Add(collision.gameObject);
            }
            StartAttack();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if(targetEnemy != null) //If we are targetting something
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
        if(attackCoroutine == null) //If the attack coroutine isnt running, start it.
        {
            attackCoroutine = StartCoroutine(AttackLoop());
        }
    }

    private IEnumerator AttackLoop()
    {
        //Checks to ensure there is a target to hit
        while (targetEnemy != null)
        {
            //Checks if the target enemy has been killed, and if so, adds a new target from the list
            if(targetEnemy == null && targets.Count > 0)
            {
                targetEnemy = targets[0];
                targets.RemoveAt(0);
            }
            else if (targetEnemy == null) 
            {
                //Ends the ienumerator as there are no targets to hit
                break;
            }

            if(stunned) //TODO: add some sort of stun time.
            {
                yield return new WaitForSeconds(3f);
                continue;
            }

            //Spawn projectile with stats
            //TODO: For optimization turn projs off and on instead of destroying
            GameObject proj = Instantiate(projectile);

            //Set the position to the towers location
            proj.transform.position = transform.position;

            //Initialize the projectile
            proj.GetComponent<Projectile>().InitializeProjectile(targetEnemy, towerDamage);

            //Activate cooldown
            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
