using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private InputAction basicAttackAction;

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

        //Subscribes to the interact input
        interactAction.started += playerInteraction.HandleInteractStarted;
        interactAction.canceled += playerInteraction.HandleInteractCancelled;
        
        //Subscribes to the basic attack input
        basicAttackAction.started += playerAbilities.TryUseBasicAttack;

    }
}
