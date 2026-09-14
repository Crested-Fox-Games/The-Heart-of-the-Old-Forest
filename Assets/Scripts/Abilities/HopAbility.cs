using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HopAbility : Ability
{
    /// <summary>
    /// The coroutine for the passive to ensure no doubling up
    /// </summary>
    private Coroutine passiveCoroutine;

    /// <summary>
    /// The current time on the passive
    /// </summary>
    private float currentPassiveTimer = 0f;

    /// <summary>
    /// The max time since last trigger for the passive
    /// </summary>
    private float passiveActiveTime = 3f;

    /// <summary>
    /// The mult we add to the players movement speed for the passive
    /// </summary>
    private float speedMult = 0.5f;


    private float forwardForce = 0.2f;

    private float upwardForce = 0.3f;

    private float AOE = 3f;

    private float damage = 50f;

    protected override void Activate()
    {
        Vector3 launchVelocity = transform.forward * forwardForce;
        launchVelocity.y = upwardForce;

        owner.GetComponent<PlayerMovement>().Launch(launchVelocity);

        owner.GetComponent<PlayerMovement>().OnLaunchLanded += OnLanded;
    }

    private void OnLanded()
    {
        List<Enemy> enemiesHit = GetHitEnemies();

        foreach (Enemy enemy in enemiesHit)
        {
            enemy.TakeDamage(owner.GetDamage(abilitySO, damage));
        }

        owner.GetComponent<PlayerMovement>().OnLaunchLanded -= OnLanded;
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


    public override void PassiveAbilitySetup()
    {
        //Subscribes the passive to its trigger
        owner.GetComponent<PlayerMovement>().onPlayerJump += PassiveTriggered;
    }

    protected override void PassiveTriggered()
    {
        //Resets the current passive time
        currentPassiveTimer = 0f;

        if(passiveCoroutine == null)
        {
            passiveCoroutine = StartCoroutine(PassiveEffect(owner.GetComponent<PlayerMovement>()));
        }
    }

    /// <summary>
    /// The Ienumerator that handles the effect of the passive
    /// </summary>
    /// <param name="playerMovement"></param>
    /// <returns></returns>
    private IEnumerator PassiveEffect(PlayerMovement playerMovement)
    {
        //Increase the player speed
        playerMovement.SetMovementSpeedMultiplier(playerMovement.MovementSpeedMultiplier +  speedMult);

        //Loops through while the timer is below the active time
        while (currentPassiveTimer < passiveActiveTime)
        {
            currentPassiveTimer += Time.deltaTime;

            yield return null;
        }

        //Decrease the player speed
        playerMovement.SetMovementSpeedMultiplier(playerMovement.MovementSpeedMultiplier - speedMult);

        passiveCoroutine = null;
    }
}
