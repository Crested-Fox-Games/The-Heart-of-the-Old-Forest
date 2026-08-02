using UnityEngine;

public abstract class Ability
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
    private float cooldownRemaining;

    public Ability(PlayerAbilities player)
    {
        owner = player;
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

        cooldownRemaining = abilitySO.Cooldown;
    }

    /// <summary>
    /// This method is delcared in the children that inherit from this class.
    /// This way we can create any ability we want from it.
    /// </summary>
    protected abstract void Activate();

    /// <summary>
    /// Ticks down the cooldown for the ability
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= deltaTime;
        }
    }
}
