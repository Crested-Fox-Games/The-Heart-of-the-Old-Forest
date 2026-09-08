using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RicochetProjectile : BaseProjectile
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
    /// A list of targets the projectile has already hit
    /// </summary>
    private List<GameObject> hitTargets = new();

    /// <summary>
    /// The maximum number of times the projectile can ricochet
    /// </summary>
    private int maxRicochets = 1;

    /// <summary>
    /// The current number of times the projectile has ricocheted
    /// </summary>
    private int currentRicochets = 0;

    /// <summary>
    /// The max distance the projectile checks for a new target to ricochet to
    /// </summary>
    private float maxRicochetDistance = 1f;

    /// <summary>
    /// Coroutine that handles the movement of the projectile towards the target position
    /// </summary>
    private Coroutine movementCoroutine;

    public override void InitializeProjectile(Vector3 target, float projectileDamage, float maxRicochetDist, int maxRicochets)
    {
        targetPosition = target;
        projDamage = projectileDamage;
        this.maxRicochets = maxRicochets;
        this.maxRicochetDistance = maxRicochetDist;
        hitTargets.Clear();

        direction = (targetPosition - transform.position).normalized;
        movementCoroutine = StartCoroutine(MoveToTarget());
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

    protected override void HandleProjectileBlightNodeHit(BlightNode blightNode)
    {
        hitTargets.Add(blightNode.gameObject);
        blightNode.TakeDamage(projDamage);
        StopCoroutine(movementCoroutine);

        if (currentRicochets < maxRicochets)
        {
            Ricochet();
        }
        else
        {
            HandleProjectileFinished();
        }
    }

    protected override void HandleProjectileEnemyHit(Enemy enemy)
    {
        hitTargets.Add(enemy.gameObject);
        enemy.TakeDamage(projDamage);
        StopCoroutine(movementCoroutine);

        if (currentRicochets <= maxRicochets)
        {
            Ricochet();
        }
        else
        {
            HandleProjectileFinished();
        }
    }

    /// <summary>
    /// Handles the logic for checking if the projectile can ricochet to a new target
    /// </summary>
    private void Ricochet()
    {
        //List of possible targets to ricochet to
        List<GameObject> possibleTargets = new List<GameObject>();

        //Populate the list of possible targets
        //Select: Selects the game object from the collider
        //Where: Checks to see if the game object has an enemy or blight node component that are not in the hitTargets list
        //OrderBy: Orders the list by distance from the projectile
        possibleTargets = Physics.OverlapSphere(transform.position, maxRicochetDistance)
            .Select(collider => collider.GetComponentInParent<Enemy>()?.gameObject ?? collider.GetComponentInParent<BlightNode>()?.gameObject)
            .Where(go => (go != null && !hitTargets.Contains(go)))
            .Distinct()
            .OrderBy(go => (go.transform.position - transform.position).sqrMagnitude)
            .ToList();

        if (possibleTargets.Count > 0)
        {
            currentRicochets++;
            //Set the new target position and direction and move the projectile towards it
            targetPosition = possibleTargets[0].transform.position;
            targetPosition.y = transform.position.y;
            direction = (targetPosition - transform.position).normalized;

            Debug.DrawRay(transform.position, direction * 5f, Color.red, 2f);
            Debug.Log($"Target enemy {possibleTargets[0]} Target Pos {targetPosition} current pos {transform.position} direction {direction}");
            movementCoroutine = StartCoroutine(MoveToTarget());
        }
        else
        {
            HandleProjectileFinished();
        }
    }
}
