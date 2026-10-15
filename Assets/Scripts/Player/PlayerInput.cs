using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    #region Player Input Variables
    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private InputAction basicAttackAction, movementAbilityAction, specialAbilityAction, ultimateAbilityAction;

    private InputAction tutorialOverlayAction;

    private PlayerInteraction playerInteraction;

    private PlayerAbilities playerAbilities;

    #endregion

    #region Ui Input Variables

    private InputActionMap uiMap;

    private InputAction ExitAction;

    #endregion

    private void Start()
    {
        //Gets references to the other scripts
        playerInteraction = GetComponent<PlayerInteraction>();
        playerAbilities = GetComponent<PlayerAbilities>();

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");
        playerMap.Enable();

        uiMap = InputSystem.actions.FindActionMap("UI");

        SubscribeToActions();
    }

    /// <summary>
    /// Allows scripts like player status to disable the player map
    /// </summary>
    public void DisablePlayerMap()
    {
        playerMap.Disable();
    }

    /// <summary>
    /// Allows scripts like player status to enable the player map
    /// </summary>
    public void EnablePlayerMap()
    {
        playerMap.Enable();
    }

    private void SubscribeToActions()
    {
        //Finds the different player inputs
        interactAction = playerMap.FindAction("Interact");
        basicAttackAction = playerMap.FindAction("BasicAttack");
        movementAbilityAction = playerMap.FindAction("MovementAbility");
        specialAbilityAction = playerMap.FindAction("SpecialAbility");
        ultimateAbilityAction = playerMap.FindAction("UltimateAbility");

        tutorialOverlayAction = playerMap.FindAction("TutorialOverlay");

        //Subscribes to the interact input
        interactAction.started += playerInteraction.HandleInteractStarted;
        interactAction.canceled += playerInteraction.HandleInteractCancelled;

        tutorialOverlayAction.started += UiManager.Instance.OpenTutorialOverlayUi;
        tutorialOverlayAction.canceled += UiManager.Instance.CloseTutorialOverlayUI;

        //Subscribes to the ability inputs
        basicAttackAction.started += playerAbilities.TryUseBasicAttack;
        movementAbilityAction.started += playerAbilities.TryUseMovementAbility;
        specialAbilityAction.started += playerAbilities.TryUseSpecialAbility;
        ultimateAbilityAction.started += playerAbilities.TryUseUltimateAttack;

        ExitAction = uiMap.FindAction("Exit");
        ExitAction.started += UiManager.Instance.HandleExitButtonPressed;

    }

    private void OnDestroy()
    {
        //unsubscribes to the interact input
        interactAction.started -= playerInteraction.HandleInteractStarted;
        interactAction.canceled -= playerInteraction.HandleInteractCancelled;

        tutorialOverlayAction.started -= UiManager.Instance.OpenTutorialOverlayUi;
        tutorialOverlayAction.canceled -= UiManager.Instance.CloseTutorialOverlayUI;

        //unsubscribes to the ability inputs
        basicAttackAction.started -= playerAbilities.TryUseBasicAttack;
        movementAbilityAction.started -= playerAbilities.TryUseMovementAbility;
        specialAbilityAction.started -= playerAbilities.TryUseSpecialAbility;
        ultimateAbilityAction.started -= playerAbilities.TryUseUltimateAttack;
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
