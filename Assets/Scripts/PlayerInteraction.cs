using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : NetworkBehaviour
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

    /// <summary>
    /// Stores the device type that was last used, e.g. kbm, controller, etc
    /// </summary>
    private InputDevice lastUsedInputDevice;

    private IInteractable currentInteractHover = null;

    private Camera playerCam;

    [SerializeField]
    private float interactDistance = 5f;

    private void Start()
    {
        lastUsedInputDevice = InputSystem.GetDevice<Keyboard>();

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");

        //This fires whenever any action on the player map is triggered
        playerMap.actionTriggered += UpdateInputDevice;

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

    /// <summary>
    /// This function checks if the player is looking at an interactable object
    /// </summary>
    private void CheckInteractableCollision()
    {
        // Check if the player is looking at an interactable object
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance))
        {
            //Checks to see if the player is looking at an interactable object(In parent checks up the hierarchy for the object)
            currentInteractHover = hit.transform.GetComponentInParent<IInteractable>();

            if (currentInteractHover != null)
            {
                //This gets the input for the interact action based on the last input device detected
                UiManager.instance.ShowInteractionPopup($"'{interactAction.GetBindingDisplayString(group: GetCurrentBindingGroup())}'");
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

    /// <summary>
    /// This function tells the object the player is trying to interact with to run its interaction function,
    /// for most objects this should include a validation check with the host.
    /// </summary>
    /// <param name="context"></param>
    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (!hoverInteracting)
            return;

        //Gets the network object of the interactable object
        NetworkObject networkObject = (currentInteractHover as MonoBehaviour).GetComponent<NetworkObject>();

        if(networkObject == null)
            return;

        InteractServerRPC(networkObject);
    }

    /// <summary>
    /// This function is called when a player interacts with an object.
    /// It will only run on the server, but each client is able to call it
    /// </summary>
    /// <param name="target"></param>
    [ServerRpc]
    private void InteractServerRPC(NetworkObject target)
    {
        //Ensures the object is valid
        if(target == null)
            return;

        //Ensures the object is still interactable
        if (!target.TryGetComponent<IInteractable>(out var interactable))
            return;

        //Ensures the player is still within range of the object
        if (Vector3.Distance(playerCam.transform.position, target.transform.position) > interactDistance)
            return;

        interactable.Interact(this);
    }

    /// <summary>
    /// This function is called whenever any input is detected,
    /// it is used to display the correct input key/icon for the interaction popup
    /// </summary>
    /// <param name="context"></param>
    private void UpdateInputDevice(InputAction.CallbackContext context)
    {
        if(lastUsedInputDevice == context.control.device)
            return;

        lastUsedInputDevice = context.control.device;

    }

    /// <summary>
    /// Returns the current binding group based on the last input received
    /// </summary>
    /// <returns></returns>
    private string GetCurrentBindingGroup()
    {
        //If we want to add support for other input devices, they just need to be added as an else if below gamepad
        if(lastUsedInputDevice is Gamepad)
            return "Gamepad";

        return "Keyboard&Mouse";
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
