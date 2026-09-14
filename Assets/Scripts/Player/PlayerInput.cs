using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private InputAction basicAttackAction, movementAbilityAction, specialAbilityAction, ultimateAbilityAction;

    private PlayerInteraction playerInteraction;

    private PlayerAbilities playerAbilities;

    private void Start()
    {
        //Gets references to the other scripts
        playerInteraction = GetComponent<PlayerInteraction>();
        playerAbilities = GetComponent<PlayerAbilities>();

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");
        
        SubscribeToActions();
    }

    private void SubscribeToActions()
    {
        //Finds the different player inputs
        interactAction = playerMap.FindAction("Interact");
        basicAttackAction = playerMap.FindAction("BasicAttack");
        movementAbilityAction = playerMap.FindAction("MovementAbility");
        specialAbilityAction = playerMap.FindAction("SpecialAbility");
        ultimateAbilityAction = playerMap.FindAction("UltimateAbility");

        //Subscribes to the interact input
        interactAction.started += playerInteraction.HandleInteractStarted;
        interactAction.canceled += playerInteraction.HandleInteractCancelled;
        
        //Subscribes to the ability inputs
        basicAttackAction.started += playerAbilities.TryUseBasicAttack;
        movementAbilityAction.started += playerAbilities.TryUseMovementAbility;
        specialAbilityAction.started += playerAbilities.TryUseSpecialAbility;
        ultimateAbilityAction.started += playerAbilities.TryUseUltimateAttack;

    }

    public InputAction GetHotkeyFromSlot(AbilitySlot abilitySlot)
    {
        return abilitySlot switch
        {
            AbilitySlot.BasicAttack => basicAttackAction,
            AbilitySlot.MovementAbility => movementAbilityAction,
            AbilitySlot.SpecialAbility => specialAbilityAction,
            AbilitySlot.UltimateAbility => ultimateAbilityAction,
            _ => throw new System.ArgumentOutOfRangeException(nameof(abilitySlot), abilitySlot, null)
        };
    }
}
