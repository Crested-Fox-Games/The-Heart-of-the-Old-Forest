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

public class ResourceNode : MonoBehaviour
{
    [SerializeField]
    private ResourceSO resourceSO;

    private ResourceType resourceType;
    private ToolTier toolTierRequired;

    private string resourceName, resourceDescription;

    private float resourceDurability;

    private float currentResourceDurability = 0;

    private void Start()
    {
        InitializeValues();

    }

    /// <summary>
    /// Sets the initial values of the node based on the SO
    /// </summary>
    private void InitializeValues()
    {
        resourceType = resourceSO.ResourceType;
        toolTierRequired = resourceSO.ToolTier;

        resourceName = resourceSO.ResourceName;
        resourceDescription = resourceSO.Description;

        resourceDurability = resourceSO.ResourceDurability;
        currentResourceDurability += resourceDurability;
    }

    //TODO: will need to pass in the player breaking it so that it knows which player to give the resources to
    //TODO: decide if tools will have durability
    //TODO: decide if resources are dropped on node destruction, or every hit
    //TODO: decide if nodes respawn or if theyre one time, if respawn, how often
    public bool Breakable(ToolTier equippedToolTier)
    {
        if(equippedToolTier >= toolTierRequired)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DamageNode()
    {
        //Reduces the durability left on the resource
        currentResourceDurability--;

        //Checks if the resource should be destroyed/disabled
        if (currentResourceDurability <= 0)
        {
            //Nothing left in resource
           
        }
    }

}
