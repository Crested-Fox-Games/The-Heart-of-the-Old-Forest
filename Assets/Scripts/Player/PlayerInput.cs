using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private PlayerInteraction playerInteraction;

    private void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");
        
        SubscribeToActions();
    }

    private void SubscribeToActions()
    {
        //Finds the different player inputs
        interactAction = playerMap.FindAction("Interact");

        //Subscribes to the interact input
        interactAction.started += playerInteraction.HandleInteractStarted;
        interactAction.canceled += playerInteraction.HandleInteractCancelled;
    }
}
