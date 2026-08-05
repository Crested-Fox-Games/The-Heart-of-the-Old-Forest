using FishNet.Object;
using FishNet.Object.Synchronizing;
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


    public readonly SyncVar<bool> depleted = new();

    /// <summary>
    /// The time it takes for the node to respawn after being depleted
    /// </summary>
    private float respawnTime = 5f;

    [SerializeField]
    private GameObject model;

    private void Start()
    {
        InitializeValues();

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        depleted.OnChange += OnDepletedChanged;
    }

    override public void OnStopClient()
    {
        base.OnStopClient();

        depleted.OnChange -= OnDepletedChanged;
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

        //Bools
        depleted.Value = false;
    }

    /// <summary>
    /// Checks if the node is breakable with the current tool
    /// </summary>
    /// <param name="equippedToolTier">The players tool tier</param>
    /// <returns>True for player damaging node, false for tool too weak</returns>
    public bool Breakable(ToolTier equippedToolTier)
    {
        //Return early if the resource is depleted
        if(depleted.Value)
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
        if(depleted.Value)
            return 0;

        //Reduces the durability left on the resource
        currentResourceDurability--;

        //Checks if the resource should be destroyed/disabled
        if (currentResourceDurability <= 0)
        {
            //Disables hitting the node
            depleted.Value = true;

            //This shouldnt need a validation check since this function shouldnt run once the node is depleted
            StartCoroutine(NodeRespawn());
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
        return !depleted.Value;
    }

    /// <summary>
    /// Handles respawning the node after it has been depleted
    /// </summary>
    /// <returns></returns>
    private IEnumerator NodeRespawn()
    { //TODO: This may need to be put into a node controller that handles respawning nodes in an area
        
        yield return new WaitForSeconds(respawnTime);

        //Resets the node values
        currentResourceDurability = resourceDurability;
        depleted.Value = false;
        model.SetActive(true);
    }

    private void OnDepletedChanged(bool oldValue, bool newValue, bool asServer)
    {
        if (newValue)
        {
            //Node is depleted
            model.SetActive(false);
        }
        else
        {
            //Node is respawned
            model.SetActive(true);
        }
    }
}
