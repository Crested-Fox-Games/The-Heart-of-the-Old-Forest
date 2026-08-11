using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : NetworkBehaviour
{
    /// <summary>
    /// A boolean used to check if the player is looking at something that can be interacted with
    /// </summary>
    private bool hoverInteracting = false;

    /// <summary>
    /// A boolean used to see if the player is holding down the interact button
    /// </summary>
    private bool interactHeld = false;

    /// <summary>
    /// The distance from the player to the interactable that the player can interact within
    /// </summary>
    [SerializeField]
    private float interactDistance = 5f;

    /// <summary>
    /// The rate at which the player interacts with objects
    /// </summary>
    [SerializeField]
    private float interactRate = 0.5f;

    /// <summary>
    /// The time the player interacts at
    /// </summary>
    private float nextInteractTime;

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
    private LayerMask aimMask;

    /// <summary>
    /// The resources the player has on hand
    /// <para>The resource type is the key, the int is the amount of that resource</para>
    /// </summary>
    public readonly SyncDictionary<ResourceType, int> resourceAmounts = new();

    private void Start()
    {
        lastUsedInputDevice = InputSystem.GetDevice<Keyboard>();

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");

        //Finds the different player inputs
        interactAction = playerMap.FindAction("Interact");

        //This fires whenever any action on the player map is triggered
        playerMap.actionTriggered += UpdateInputDevice;
    }

    override public void OnStartClient()
    {
        base.OnStartClient();

        if(IsOwner)
            resourceAmounts.OnChange += OnResourcedChanged;
    }

    override public void OnStopClient()
    {
        base.OnStopClient();

        if(IsOwner)
            resourceAmounts.OnChange -= OnResourcedChanged;
    }

    private void Update()
    {
        CheckInteractableCollision();

        CheckInteractLoop();
    }

    /// <summary>
    /// A function that checks if the player is holding down the interact button
    /// </summary>
    private void CheckInteractLoop()
    {
        if (!interactHeld)
            return;

        if (Time.time < nextInteractTime)
            return;

        nextInteractTime = Time.time + interactRate;

        TryInteract();
    }

    /// <summary>
    /// This function checks if the player is looking at an interactable object
    /// </summary>
    private void CheckInteractableCollision()
    {
        if (playerCam == null)
            return;

        // Check if the player is looking at an interactable object
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            //Checks to see if the player is looking at an interactable object(In parent checks up the hierarchy for the object)
            currentInteractHover = hit.transform.GetComponentInParent<IInteractable>();

            if (currentInteractHover != null)
            {
                //This gets the input for the interact action based on the last input device detected
                UiManager.Instance.ShowInteractionPopup($"'{interactAction.GetBindingDisplayString(group: GetCurrentBindingGroup())}'");
                hoverInteracting = true;
            }
            else //This will run if the player is looking at something that cant be interacted with
            {
                if(!hoverInteracting)
                    return;

                //This is here incase the player goes from looking at something interactable to something not interactable
                //that way the popup will disappear
                UiManager.Instance.HideInteractionPopup();
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
            UiManager.Instance.HideInteractionPopup();
        }
    }

    
    public void HandleInteractStarted(InputAction.CallbackContext context)
    {
        interactHeld = true;

        nextInteractTime = Time.time + interactRate;
    }

    public void HandleInteractCancelled(InputAction.CallbackContext context)
    {
        interactHeld = false;
    }

    /// <summary>
    /// This function tells the object the player is trying to interact with to run its interaction function,
    /// for most objects this should include a validation check with the host.
    /// </summary>
    /// <param name="context"></param>
    private void TryInteract()
    {
        if (!hoverInteracting)
            return;

        //Gets the network object of the interactable object
        NetworkObject networkObject = (currentInteractHover as MonoBehaviour).GetComponent<NetworkObject>();

        if (networkObject == null)
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
        {
            Debug.LogWarning($"Player {Owner.ClientId} tried to interact with {target.name}, but it is no longer interactable");
            return;
        }
            

        //Ensures the player is still within range of the object
        if (Vector3.Distance(transform.position, target.transform.position) > interactDistance)
        {
            Debug.LogWarning($"Player {Owner.ClientId} tried to interact with {target.name}, but they are too far away");
            return;
        }

        //Does a final check to ensure if the player meets the conditions to interact with the object
        if (!interactable.CanInteract(this))
        {
            Debug.LogWarning($"Player {Owner.ClientId} tried to interact with {target.name}, but they do not meet the conditions to interact");
            return;
        }

        interactable.Interact(this);
        
    }

    /// <summary>
    /// This is called by the resource node when the player collects a resource
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="amount"></param>
    public void AcquireResources(ResourceType resourceType, int amount)
    {
        if(resourceAmounts.TryGetValue(resourceType, out var current))
        {
            resourceAmounts[resourceType] = amount + current;
        }
        else
        {
            resourceAmounts.Add(resourceType, amount);
        }
    }

    [ServerRpc]
    public void DepositResources(NetworkObject resourceController)
    {
        //This will be where the player deposits their resources into the team stockpile
        //This will need to be a server rpc that sends the resource amounts to the team stockpile
        //Then clears the players resource amounts
        var controller = resourceController.GetComponentInChildren<BaseResourceController>();

        if (controller == null)
            return;

        foreach (var resource in resourceAmounts.ToList())
        {
            controller.AddResources(resource.Key, resource.Value);
            resourceAmounts[resource.Key] = 0;
        }
    }

    private void OnResourcedChanged(SyncDictionaryOperation op, ResourceType key, int value, bool asServer)
    {

        Debug.Log($"Player {Owner.ClientId} resource {key} changed to {value}");
        UiManager.Instance.UpdatePlayerResourceUi(resourceAmounts);
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
