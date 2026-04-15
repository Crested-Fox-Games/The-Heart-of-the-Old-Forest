using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone
}

[CreateAssetMenu(fileName = "ResourceSO", menuName = "Resources/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    [SerializeField]
    private ResourceType resourceType;
    [SerializeField]
    private ToolTier toolTierRequired;
    [SerializeField]
    private string resourceName, resourceDescription;
    [SerializeField]
    private float resourceDurability;
    [SerializeField]
    private int resourceamountDropped;

    /// <summary>
    /// The resources type
    /// </summary>
    public ResourceType ResourceType => resourceType;

    /// <summary>
    /// The tool required to collect the resource
    /// </summary>
    public ToolTier ToolTier => toolTierRequired;

    /// <summary>
    /// The name of the resource
    /// </summary>
    public string ResourceName => resourceName;

    /// <summary>
    /// The description of the resource
    /// </summary>
    public string Description => resourceDescription;

    /// <summary>
    /// The amount of times the node needs to be hit before its destroyed
    /// </summary>
    public float ResourceDurability => resourceDurability;

    /// <summary>
    /// The amount of resources dropped - TODO: specify whether per hit or for whole node
    /// </summary>
    public int ResourceAmountDropped => resourceamountDropped;
}
