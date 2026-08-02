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

    public float CooldownRemaining => cooldownRemaining;

    public AbilitySO AbilitySO => abilitySO;

    public Ability(PlayerAbilities player, AbilitySO abilityData)
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

        cooldownRemaining = abilitySO.Cooldown;
    }

    /// <summary>
    /// Check to see if we are able to use the ability
    /// Overload that allows for a direction to be passed in
    /// </summary>
    public void UseAbility(Vector3 direction)
    {
        Activate();

        cooldownRemaining = abilitySO.Cooldown;
    }

    /// <summary>
    /// This method is delcared in the children that inherit from this class.
    /// This way we can create any ability we want from it.
    /// </summary>
    protected virtual void Activate()
    {

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
