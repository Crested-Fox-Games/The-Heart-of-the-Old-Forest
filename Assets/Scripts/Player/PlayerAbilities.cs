using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public enum AbilitySlot
{
    BasicAttack,
    FirstAbility,
    SecondAbility
}

public class PlayerAbilities : NetworkBehaviour
{

    private Ability basicAttack, firstAbility, secondAbility;

    private void Awake()
    {
        basicAttack = new RabbitBasicAttack(this);
    }

    private void TryUseAbility(AbilitySlot abilitySlot)
    {
        UseAbility(abilitySlot);
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

    public void TryUseBasicAttack()
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
}
