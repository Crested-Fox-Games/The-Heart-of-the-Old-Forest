using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public enum AbilitySlot
{
    BasicAttack,
    MovementAbility,
    SpecialAbility,
    UltimateAbility
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
    private AbilitySO basicAttackSO, movementAbilitySO, specialAbilitySO, ultimateAbilitySO;
    private Ability basicAttack, movementAbility, specialAbility, ultimateAbility;

    public Ability BasicAttack => basicAttack;

    /// <summary>
    /// A dictionary that holds the values for the abilities upgrades
    /// </summary>
    private readonly SyncDictionary<AbilitySlot, AbilityUpgradesDC> abilityUpgrades = new();

    /// <summary>
    /// This syncvar is used to let the client know how long is left on the cooldown, used for UI purposes
    /// </summary>
    private readonly SyncVar<float> basicAttackCooldownRemaining = new SyncVar<float>();
    private readonly SyncVar<float> movementAbilityCooldownRemaining = new SyncVar<float>();
    private readonly SyncVar<float> specialAbilityCooldownRemaining = new SyncVar<float>();
    private readonly SyncVar<float> ultimateAbilityCooldownRemaining = new SyncVar<float>();

    /// <summary>
    /// This is the projectile for the player basic attack.
    /// This will probably need to be changed later for when we have multiple different characters
    /// </summary>
    [SerializeField]
    private BaseProjectile projectile;

    public BaseProjectile Projectile => projectile;

    [SerializeField]
    private Transform firingPosition;

    public Transform FiringPosition => firingPosition;

    [SerializeField]
    private LayerMask aimMask;

    private void Awake()
    {
        basicAttack = AddAbility(basicAttackSO);
        movementAbility = AddAbility(movementAbilitySO); //NOT IMPLEMENTED
        specialAbility = AddAbility(specialAbilitySO);
        ultimateAbility = AddAbility(ultimateAbilitySO);
        
    }

    private void Update()
    {
        if (!IsServerStarted)
            return;

        //Ticks down the basic attacks cooldown timer
        basicAttack.Tick(Time.deltaTime);
        movementAbility.Tick(Time.deltaTime);
        specialAbility.Tick(Time.deltaTime);
        ultimateAbility.Tick(Time.deltaTime);

        //Updates the sync var to let the client know how much time is left on the cooldown
        basicAttackCooldownRemaining.Value = basicAttack.CooldownRemaining;
        movementAbilityCooldownRemaining.Value = movementAbility.CooldownRemaining;
        specialAbilityCooldownRemaining.Value = specialAbility.CooldownRemaining;
        ultimateAbilityCooldownRemaining.Value = ultimateAbility.CooldownRemaining;
    }

    private Ability AddAbility(AbilitySO abilitySO)
    {
        Type abilityType = Type.GetType(abilitySO.AbilityTypeName);

        if(abilityType == null)
        {
            Debug.LogError($"Could not find ability type '{abilitySO.AbilityTypeName}' for ability '{abilitySO.AbilityName}'");

            return null;
        }

        Ability ability = gameObject.AddComponent(abilityType) as Ability;

        if(ability == null)
        {
            Debug.LogError($"Type '{abilityType}' does not inherit from ability");
        }

        ability.Initialize(this, abilitySO);

        return ability;
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

        if (!ability.CanUseAbility())
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

    public void TryUseMovementAbility(InputAction.CallbackContext context)
    {
        TryUseAbility(AbilitySlot.MovementAbility);
    }

    public void TryUseSpecialAbility(InputAction.CallbackContext context)
    {
        TryUseAbility(AbilitySlot.SpecialAbility);
    }

    public void TryUseUltimateAttack(InputAction.CallbackContext context)
    {
        TryUseAbility(AbilitySlot.UltimateAbility);
    }

    /// <summary>
    /// Gets the ability from the specified slot.
    /// </summary>
    /// <param name="abilitySlot"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public Ability GetAbilityFromSlot(AbilitySlot abilitySlot)
    {
        //A switch statement which returns the ability in the specified slot
        return abilitySlot switch
        {
            AbilitySlot.BasicAttack => basicAttack,
            AbilitySlot.MovementAbility => movementAbility,
            AbilitySlot.SpecialAbility => specialAbility,
            AbilitySlot.UltimateAbility => ultimateAbility,
            _ => throw new System.ArgumentOutOfRangeException(nameof(abilitySlot), abilitySlot, null)
        };
    }

    /// <summary>
    /// Gets the ability slot from the specified ability.
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException"></exception>
    public AbilitySlot GetSlotFromAbility(AbilitySO ability)
    {
        //This is basically a streamlined if statement that checks to see if the ability matches the one in each slot
        //The _ checks to see if it matches anything, and then the when checks to see if the boolean expression is true
        return ability switch
        {
            _ when ability == basicAttack.AbilitySO => AbilitySlot.BasicAttack,
            _ when ability == movementAbility.AbilitySO => AbilitySlot.MovementAbility,
            _ when ability == specialAbility.AbilitySO => AbilitySlot.SpecialAbility,
            _ when ability == ultimateAbility.AbilitySO => AbilitySlot.UltimateAbility,
            _ => throw new System.ArgumentException("Ability not found in any slot", nameof(ability))
        };
    }

    /// <summary>
    /// Returns the cooldown based on which slot is passed in
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    public SyncVar<float> GetCooldownSyncVarFromSlot(AbilitySlot slot)
    {
        return slot switch
        {
            AbilitySlot.BasicAttack => basicAttackCooldownRemaining,
            AbilitySlot.MovementAbility => movementAbilityCooldownRemaining,
            AbilitySlot.SpecialAbility => specialAbilityCooldownRemaining,
            AbilitySlot.UltimateAbility => ultimateAbilityCooldownRemaining,
            _ => throw new System.ArgumentOutOfRangeException(nameof(slot), slot, null)
        };
    }

    public float GetCooldownFromSlot(AbilitySlot slot)
    {
        return GetCooldownSyncVarFromSlot(slot).Value;
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
        BaseProjectile newProjectile = Instantiate(projectilePrefab, firingPosition.position, projRotation).GetComponent<BaseProjectile>();

        InitializeProjectiles(newProjectile, target, abilitySO, baseDamage, projectilePrefab);

        //Spawns the projectile on the network
        Spawn(newProjectile.gameObject);
    }

    //NOTE: This is so ugly, need to find a better way of doing it
    private void InitializeProjectiles(BaseProjectile newProjectile, Vector3 target, AbilitySO abilitySO, float baseDamage, GameObject projectilePrefab)
    {
        if (projectilePrefab.GetComponent<NormalProjectile>() != null)
        {
            newProjectile.InitializeProjectile(target, GetDamage(abilitySO, baseDamage));
        }
        else if (projectilePrefab.GetComponent<RicochetProjectile>() != null)
        {
            //Initializes the projectiles values
            newProjectile.InitializeProjectile(target, GetDamage(abilitySO, baseDamage), 1f, 3);
        }
        else if(projectilePrefab.GetComponent<VacuumProjectile>() != null)
        {
            newProjectile.InitializeProjectile(target, GetDamage(abilitySO, baseDamage), this);
        }

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

        //Flat reduction
        float cooldown = baseCooldown - abilityUpgrades.cooldownAdd;

        //Percentage reduction with diminishing returns
        float reduction = 1f - (1f / abilityUpgrades.cooldownMult);

        cooldown *= 1 - reduction;

        Debug.Log($"cooldown for {abilitySO.AbilityName} cooldown: {cooldown}");
        //Ensures that the cooldown never hits 0
        return Mathf.Max(0.1f, cooldown);
    }

    

}
