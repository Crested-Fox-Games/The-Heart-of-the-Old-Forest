using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// A boolean used to check if the player is looking at something that can be interacted with
    /// </summary>
    private bool hoverInteracting = false;

    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction interactAction;

    private IInteractable currentInteractHover = null;

    private Camera playerCam;

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
        if (playerCam == null)
            return;

        CheckInteractableCollision();
    }

    private void CheckInteractableCollision()
    {
        // Check if the player is looking at an interactable object
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 3f))
        {
            Debug.Log(hit.transform.gameObject.name);
            //Checks to see if the player is looking at an interactable object
            currentInteractHover = hit.transform.GetComponentInParent<IInteractable>();

            if (currentInteractHover != null)
            {
                UiManager.instance.ShowInteractionPopup($"'{interactAction.GetBindingDisplayString()}'");
                hoverInteracting = true;
            }
            else //This will run if the player is looking at something that cant be interacted with
            {
                if(!hoverInteracting)
                    return;

                //This is here incase the player goes from looking at something interactable to something not interactable
                //that way the popup will disappear
                UiManager.instance.HideInteractionPopup();
                hoverInteracting = false;
                currentInteractHover = null;
            }
        }
        else
        {
            if (!hoverInteracting)
                return;

            hoverInteracting = false;
            currentInteractHover = null;
            UiManager.instance.HideInteractionPopup();
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (!hoverInteracting)
            return;

        currentInteractHover.Interact();
    }

    /// <summary>
    /// Sets the camera for this script, called from PlayerMovement
    /// </summary>
    /// <param name="camera"></param>
    public void SetCamera(Camera camera)
    {
        playerCam = camera;
    }

}
