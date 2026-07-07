using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// A text box in the middle of the screen showing the keybind for interacting with objects
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI popupText;

    /// <summary>
    /// A boolean used to check if the player is looking at something that can be interacted with
    /// </summary>
    private bool hoverInteracting = false;

    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private GameObject currentInteractHover = null;


    private void Start()
    {
        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");

        //Finds the different player inputs
        interactAction = playerMap.FindAction("Interact");

        //Subscribes to the interact input
        interactAction.performed += HandleInteract;
    }

    private void Update()
    {
        CheckInteractableCollision();
    }

    private void CheckInteractableCollision()
    {
        // Check if the player is looking at an interactable object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (hit.transform.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                ShowInteractionPopup($"'{interactAction.GetBindingDisplayString()}'");
            }
            else
            {
                HideInteractionPopup();
            }
        }
        else
        {
            hoverInteracting = false;
            currentInteractHover = null;
            HideInteractionPopup();
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (!hoverInteracting)
            return;

        currentInteractHover.GetComponent<IInteractable>().Interact();
    }

    /// <summary>
    /// Used to display the text for the interaction popup
    /// </summary>
    /// <param name="interactionText"></param>
    private void ShowInteractionPopup(string interactionText)
    {
        popupText.text = interactionText;
        popupText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Used to hide the text for interaction popup
    /// </summary>
    private void HideInteractionPopup()
    {
        popupText.text = "";
        popupText.gameObject.SetActive(false);
    }

}
