using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum AbilitySlot
{
    BasicAttack,
    FirstAbility,
    SecondAbility
}

/// <summary>
/// The stats of the ability that can be upgraded
/// </summary>
public enum AbilityStat
{
    Damage,
    Cooldown,
    Range
}

/// <summary>
/// A data class used for the dictionary of tower upgrades
/// </summary>
[Serializable]
public struct AbilityUpgradesDC
{
    public static AbilityUpgradesDC Default => new AbilityUpgradesDC
    {
        damageAdd = 0f,
        cooldownAdd = 0f,
        rangeAdd = 0f,

        damageMult = 1f,
        cooldownMult = 1f,
        rangeMult = 1f,
    };

    //Attack Modifiers
    public float damageAdd;
    public float damageMult;

    //Attack Speed Modifiers
    public float cooldownAdd;
    public float cooldownMult;

    //Range Modifiers
    public float rangeAdd;
    public float rangeMult;
}

public class PlayerAbilities : NetworkBehaviour
{
    [SerializeField]
    private AbilitySO basicAttackSO, firstAbilitySO, secondAbilitySO;
    private Ability basicAttack, firstAbility, secondAbility;

    /// <summary>
    /// A dictionary that holds the values for the abilities upgrades
    /// </summary>
    private readonly SyncDictionary<AbilitySlot, AbilityUpgradesDC> abilityUpgrades = new();

    /// <summary>
    /// This syncvar is used to let the client know how long is left on the cooldown, used for UI purposes
    /// </summary>
    private readonly SyncVar<float> basicAttackCooldownRemaining = new SyncVar<float>();

    /// <summary>
    /// This is the projectile for the player basic attack.
    /// This will probably need to be changed later for when we have multiple different characters
    /// </summary>
    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    private Transform firingPosition;

    [SerializeField]
    private LayerMask aimMask;

    private void Awake()
    {
        basicAttack = new RabbitBasicAttack(this, basicAttackSO, projectile);

    }

    private void Update()
    {
        if (!IsServerStarted)
            return;

        //Ticks down the basic attacks cooldown timer
        basicAttack.Tick(Time.deltaTime);

        //Updates the sync var to let the client know how much time is left on the cooldown
        basicAttackCooldownRemaining.Value = basicAttack.CooldownRemaining;
    }

    private void TryUseAbility(AbilitySlot abilitySlot)
    {
       if(!IsOwner)
            return;

        if (GetAbilityFromSlot(abilitySlot).AbilitySO.NeedDirection)
        {
            UseAbility(abilitySlot, GetAimPoint());
        }
        else
        {
            UseAbility(abilitySlot);
        }
    }


    /// <summary>
    /// The server validates to see if the ability can be used and then uses it if it can
    /// </summary>
    /// <param name="abilitySlot"></param>
    [ServerRpc]
    private void UseAbility(AbilitySlot abilitySlot)
    {
        Ability ability = GetAbilityFromSlot(abilitySlot);

        if (ability == null)
            return;

        if(!ability.CanUseAbility())
            return;

        ability.UseAbility();
    }

    [ServerRpc]
    private void UseAbility(AbilitySlot abilitySlot, Vector3 direction)
    {
        Ability ability = GetAbilityFromSlot(abilitySlot);

        if (ability == null)
            return;

        if(!ability.CanUseAbility())
            return;

        ability.UseAbility(direction);
    }

    public void TryUseBasicAttack(InputAction.CallbackContext context)
    {
        TryUseAbility(AbilitySlot.BasicAttack);
    }

    //public void TryUseFirstAbility()
    //{
    //    TryUseAbility(AbilitySlot.FirstAbility);
    //}

    //public void TryUseSecondAbility()
    //{
    //    TryUseAbility(AbilitySlot.SecondAbility);
    //}

    /// <summary>
    /// Gets the ability from the specified slot.
    /// </summary>
    /// <param name="abilitySlot"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    private Ability GetAbilityFromSlot(AbilitySlot abilitySlot)
    {
        //A switch statement which returns the ability in the specified slot
        return abilitySlot switch
        {
            AbilitySlot.BasicAttack => basicAttack,
            AbilitySlot.FirstAbility => firstAbility,
            AbilitySlot.SecondAbility => secondAbility,
            _ => throw new System.ArgumentOutOfRangeException(nameof(abilitySlot), abilitySlot, null)
        };
    }

    /// <summary>
    /// Gets the ability slot from the specified ability.
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException"></exception>
    private AbilitySlot GetSlotFromAbility(AbilitySO ability)
    {
        //This is basically a streamlined if statement that checks to see if the ability matches the one in each slot
        //The _ checks to see if it matches anything, and then the when checks to see if the boolean expression is true
        return ability switch
        {
            _ when ability == basicAttack.AbilitySO => AbilitySlot.BasicAttack,
            _ when ability == firstAbility.AbilitySO => AbilitySlot.FirstAbility,
            _ when ability == secondAbility.AbilitySO => AbilitySlot.SecondAbility,
            _ => throw new System.ArgumentException("Ability not found in any slot", nameof(ability))
        };
    }

    /// <summary>
    /// Gets the point at which the player is aiming at
    /// </summary>
    /// <returns></returns>
    private Vector3 GetAimPoint()
    {
        //Gets the players camera
        Camera playerCamera = GetComponentInChildren<Camera>();

        //Creates a ray from the cameras position in the forward direction
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        //Checks to see if the ray hits anything in the aimMask layer (ignoring triggers)
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, aimMask, QueryTriggerInteraction.Ignore))
        {
            //Returns the point at which the ray hit something
            return hit.point;
        }

        //returns a point 100 units away in the direction the player is aiming if nothing is hit
        return ray.origin + ray.direction * 100f;
    }

    /// <summary>
    /// Handles spawning projectiles for player abilities
    /// </summary>
    /// <param name="projectilePrefab"></param>
    /// <param name="target"></param>
    /// <param name="baseDamage"></param>
    /// <param name="abilitySO"></param>
    public void SpawnProjectile(GameObject projectilePrefab, Vector3 target, float baseDamage, AbilitySO abilitySO)
    {
        //Gets the direction the projectile should be facing
        Vector3 dir = (target - firingPosition.position).normalized;

        //Creates a rotation for the projectile to face the target
        Quaternion projRotation = Quaternion.LookRotation(dir);

        // Spawns the projectile on the server
        Projectile newProjectile = Instantiate(projectilePrefab, firingPosition.position, projRotation).GetComponent<Projectile>();

        //Initializes the projectiles values
        newProjectile.InitializeProjectile(target, GetDamage(abilitySO, baseDamage));

        //Spawns the projectile on the network
        Spawn(newProjectile.gameObject);
    }

    public void AddAbilityUpgrade(AbilitySO abilitySO, AbilityStats abilityStat, UpgradeType rewardType, float rewardAmount)
    {
        //Checks to ensure we are running this on the server
        if (!InstanceFinder.IsServerStarted)
            return;

        AbilityUpgradesDC upgrades = GetOrCreateGlobalUpgrades(abilitySO);

        //Adds to the upgrades
        if (rewardType == UpgradeType.Addition)
        {
            switch (abilityStat)
            {
                case AbilityStats.Damage:
                    upgrades.damageAdd += rewardAmount;
                    break;
                case AbilityStats.Cooldown:
                    upgrades.cooldownAdd += rewardAmount;
                    break;
            }
        }
        else
        {
            switch (abilityStat)
            {
                case AbilityStats.Damage:
                    upgrades.damageMult += rewardAmount;
                    break;
                case AbilityStats.Cooldown:
                    upgrades.cooldownMult += rewardAmount;
                    break;
            }
        }

        //Updates the upgrades in the dictionary
        abilityUpgrades[GetSlotFromAbility(abilitySO)] = upgrades;
    }

    /// <summary>
    /// Either creates or gets the upgrades for a specific ability
    /// </summary>
    /// <param name="abilitySO"></param>
    /// <returns></returns>
    public AbilityUpgradesDC GetOrCreateGlobalUpgrades(AbilitySO abilitySO)
    {
        //If it cant find the ability upgrades DC then it creates a default one
        if (!abilityUpgrades.TryGetValue(GetSlotFromAbility(abilitySO), out AbilityUpgradesDC upgrades))
        {
            upgrades = AbilityUpgradesDC.Default;
            abilityUpgrades.Add(GetSlotFromAbility(abilitySO), upgrades);
        }

        //Either returns the created one, or the one from the try get values out
        return upgrades;
    }


    public float GetDamage(AbilitySO abilitySO, float baseDamage)
    {
        AbilityUpgradesDC abilityUpgrades = GetOrCreateGlobalUpgrades(abilitySO);

        return (baseDamage + abilityUpgrades.damageAdd) * abilityUpgrades.damageMult;
    }

    public float GetCooldown(AbilitySO abilitySO, float baseCooldown)
    {
        AbilityUpgradesDC abilityUpgrades = GetOrCreateGlobalUpgrades(abilitySO);

        return (baseCooldown + abilityUpgrades.cooldownAdd) * abilityUpgrades.cooldownMult;
    }

}
