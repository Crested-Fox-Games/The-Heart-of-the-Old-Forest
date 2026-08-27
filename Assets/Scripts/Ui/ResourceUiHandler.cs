using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ResourceUiHandler : MonoBehaviour
{
    private Dictionary<ResourceType, ResourceIndexUi> displayedResources = new Dictionary<ResourceType, ResourceIndexUi>();

    [SerializeField]
    private GameObject resourceIndexUiPrefab;

    private void Start()
    {
        BaseResourceController.Instance.ResourceAmounts.OnChange += BaseResourceChanges;
    }

    private void BaseResourceChanges(SyncDictionaryOperation op, ResourceType key, int val, bool asServer)
    {
        UpdateOrCreateDisplayedResource(key, val);
    }

    public void UpdateOrCreateDisplayedResource(ResourceType resource, int playerResourceAmount)
    {
        if(!displayedResources.TryGetValue(resource, out ResourceIndexUi resourceIndex))
        {
            //Create the prefab and get the indexUi script
            resourceIndex = Instantiate(resourceIndexUiPrefab, transform.position, Quaternion.identity, transform).GetComponent<ResourceIndexUi>();

            //Initialize the values for the script
            resourceIndex.Initialize(resource);

            //Add the script to the dictionary
            displayedResources.Add(resource, resourceIndex);
        }

        //Update the values in the ui text
        displayedResources[resource].UpdateUiText(playerResourceAmount, BaseResourceController.Instance.GetResourceAmount(resource));
    }
}
