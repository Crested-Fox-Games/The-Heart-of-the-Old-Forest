using FishNet.Object;
using System;
using UnityEngine;

[Serializable]
public abstract class Ability : NetworkBehaviour
{
    /// <summary>
    /// Stores the abilities data
    /// </summary>
    protected AbilitySO abilitySO;

    /// <summary>
    /// The player ability of the player that owns this ability
    /// </summary>
    protected PlayerAbilities owner;

    /// <summary>
    /// Stores the cooldown for the current player
    /// </summary>
    private float cooldownRemaining = 0f;

    public float CooldownRemaining => cooldownRemaining;

    /// <summary>
    /// A bool for if the ability has an active portion
    /// </summary>
    protected bool hasActive = false;

    /// <summary>
    /// The amount of time the ability is active for
    /// </summary>
    protected float activeTimer = 0f;

    private float activeRemaining = 0f;

    private bool isActive = false;

    public AbilitySO AbilitySO => abilitySO;

    public void Initialize(PlayerAbilities player, AbilitySO abilityData)
    {
        owner = player;
        abilitySO = abilityData;
    }

    /// <summary>
    /// Function to see if the ability is available to use
    /// </summary>
    public bool CanUseAbility()
    {
        if(cooldownRemaining > 0)
            return false;


        return true;
    }

    /// <summary>
    /// Check to see if we are able to use the ability
    /// </summary>
    public void UseAbility()
    {
        Activate();

        cooldownRemaining = owner.GetCooldown(AbilitySO, AbilitySO.Cooldown);
        activeRemaining = activeTimer;
    }

    /// <summary>
    /// Check to see if we are able to use the ability
    /// Overload that allows for a direction to be passed in
    /// </summary>
    public void UseAbility(Vector3 direction)
    {
        Activate(direction);

        cooldownRemaining = owner.GetCooldown(abilitySO, abilitySO.Cooldown);
        activeRemaining = activeTimer;
    }

    /// <summary>
    /// Deactivates the active ability
    /// </summary>
    public void AbilityFinished()
    {
        Deactivate();
    }

    /// <summary>
    /// This method is delcared in the children that inherit from this class.
    /// This way we can create any ability we want from it.
    /// </summary>
    protected virtual void Activate()
    {
        if(hasActive)
        {
            isActive = true;
        }
    }

    /// <summary>
    /// An overridable method for abilities that need to know a direction
    /// </summary>
    /// <param name="aimDirection"></param>
    protected virtual void Activate(Vector3 direction)
    {
        Activate();
    }

    /// <summary>
    /// An overridable method for deactivating abilities that have an active time
    /// </summary>
    protected virtual void Deactivate()
    {
        if(hasActive)
            isActive = false;

        //Resets the cooldown again to make it easier
        cooldownRemaining = owner.GetCooldown(AbilitySO, AbilitySO.Cooldown);
    }

    /// <summary>
    /// Ticks down the cooldown for the ability
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Tick(float deltaTime)
    {
        if(isActive && activeRemaining > 0)
        {
            Debug.Log($"Active remaining {activeRemaining}");
            activeRemaining -= deltaTime;
        }
        else if(!isActive && activeRemaining > 0)
        {
            activeRemaining = 0;
        }    

        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= deltaTime;
        }
    }

    /// <summary>
    /// An overridable method for setting the projectile for an ability
    /// </summary>
    public virtual void SetProjectile(GameObject proj)
    {

    }

    public bool HasActive()
    {
        return hasActive;
    }

    public float ActiveRemaining()
    {
        return activeRemaining;
    }

    public float ActiveTime()
    {
        return activeTimer;
    }
}
