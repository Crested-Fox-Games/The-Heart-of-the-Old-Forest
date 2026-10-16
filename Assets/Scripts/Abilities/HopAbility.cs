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

        if (abilitySO.ActivationAudio != null)
        {
            AudioManager.Instance.PlayAudioClip(abilitySO.ActivationAudio, transform.position);
        }
        else
        {
            Debug.LogWarning($"The audio clip doesnt exist for {abilitySO}");
        }
    }

    private void OnLanded()
    {
        if (abilitySO.ActivationAudio != null)
        {
            AudioManager.Instance.PlayAudioClip(abilitySO.EffectAudio, transform.position);
        }
        else
        {
            Debug.LogWarning($"The audio clip doesnt exist for {abilitySO}");
        }

        if(abilitySO.EffectVFX != null)
        {
            GameObject effectVFX = Instantiate(abilitySO.EffectVFX, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            Destroy(effectVFX, 3f);
        }
        else
        {
            Debug.LogWarning($"The effect VFX doesnt exist for {abilitySO}");
        }

        List<Enemy> enemiesHit = GetHitEnemies();

        //Gets damage at the start so that if we crit, it crits them all
        float currDamage = owner.GetFinalDamage(abilitySO, damage);

        foreach (Enemy enemy in enemiesHit)
        {
            enemy.TakeDamage(currDamage);
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
        //Turn on VFX for the passive
        MovementVFXManager.Instance.EnableVFX();

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

        //Turn off VFX for the passive
        MovementVFXManager.Instance.DisableVFX();

        passiveCoroutine = null;
    }

    public override string GetStatString()
    {
        PlayerAbilities abilities = GameManager.Instance.LocalPlayer.playerAbilities;

        AbilityUpgradesDC upgradesDC = abilities.GetOrCreateGlobalUpgrades(abilitySO);

        return $"{abilitySO.AbilityName} \n" +
            $"Damage: {abilities.GetDamage(abilitySO, damage)} \n" +
            $"Cooldown: {abilities.GetCooldown(abilitySO, abilitySO.Cooldown)} \n" +
            $"Crit Chance: {upgradesDC.critChanceAdd}\n" +
            $"Crit Mult: {upgradesDC.critChanceMult}\n" +
            $"AOE: {AOE}m";
    }
}
