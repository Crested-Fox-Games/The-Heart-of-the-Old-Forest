using FishNet.Object;
using System.Collections;
using UnityEngine;

//TODO: Will need to move this to a more applicable script at some point
public enum ToolTier
{
    None = 0,
    Wood,
    Stone,
    Magic
}

//TODO: Add a node controller that handles node chunks and respawns them in a random spot in that area
public class ResourceNode : NetworkBehaviour, IInteractable
{
    #region SO Fields
    [SerializeField]
    private ResourceSO resourceSO;

    private ResourceType resourceType;
    private ToolTier toolTierRequired;

    private string resourceName, resourceDescription;

    private float resourceDurability;

    private int resourceAmountDropped;

    #endregion

    private float currentResourceDurability = 0;

    private bool depleted = false;

    private void Start()
    {
        InitializeValues();

    }

    /// <summary>
    /// Sets the initial values of the node based on the SO
    /// </summary>
    private void InitializeValues()
    {
        //Enums
        resourceType = resourceSO.ResourceType;
        toolTierRequired = resourceSO.ToolTier;

        //Strings
        resourceName = resourceSO.ResourceName;
        resourceDescription = resourceSO.Description;

        //Floats
        resourceDurability = resourceSO.ResourceDurability;
        currentResourceDurability += resourceDurability;

        //Ints
        resourceAmountDropped = resourceSO.ResourceAmountDropped;
    }

    /// <summary>
    /// Checks if the node is breakable with the current tool
    /// </summary>
    /// <param name="equippedToolTier">The players tool tier</param>
    /// <returns>True for player damaging node, false for tool too weak</returns>
    public bool Breakable(ToolTier equippedToolTier)
    {
        //Return early if the resource is depleted
        if(depleted)
        {
            return false;
        }

        if(equippedToolTier >= toolTierRequired)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Damages the node when a player successfully hits it
    /// </summary>
    /// <returns>Returns the amount of resources the player gains</returns>
    private int DamageNode()
    {
        if(depleted)
            return 0;

        //Reduces the durability left on the resource
        currentResourceDurability--;

        //Checks if the resource should be destroyed/disabled
        if (currentResourceDurability <= 0)
        {
            //TODO: Show a used resource node, maybe have multiple stages
            gameObject.SetActive(false);

            //Disables hitting the node
            depleted = true;
        }

        //Returns the resources when node broken
        return resourceAmountDropped;
    }

    public void Interact(NetworkObject player)
    {
        player.GetComponent<PlayerInteraction>().AcquireResources(resourceType, DamageNode());
    }

    public bool CanInteract(NetworkObject player)
    {
        //This will need to be some sort of check sent to the host to see if the player can interact with the node

        //TODO: Will need to add in any other checks, like player tool tier 
        return !depleted;
    }

}
