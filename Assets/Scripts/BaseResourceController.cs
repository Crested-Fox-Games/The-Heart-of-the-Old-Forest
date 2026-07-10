using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceController : NetworkBehaviour
{
    //This script will handle all the resources that are collected by the players

    /// <summary>
    /// The resources the team will have access to 
    /// <para>The resource type is the key, the int is the amount of that resource</para>
    /// </summary>
    private Dictionary<ResourceType, int> resourceAmounts = new Dictionary<ResourceType, int>();

    //This will take all of the resources the players have collected and put it into a team storage
    private void OnTriggerEnter(Collider other)
    {
        //TODO: Once player created, add this function to take the resources from them 
        //Player player;

        //Handles the player collision interaction
        if (other.TryGetComponent<PlayerInteraction>(out PlayerInteraction player))
        {
            player.DepositResources(this);
        }

    }

    /// <summary>
    /// Adds resources to the controller stockpile
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="amount"></param>
    public void AddResources(ResourceType resourceType, int amount)
    {
        //Adds resources to the stockpile
        if (resourceAmounts.TryGetValue(resourceType, out var current))
        {
            resourceAmounts[resourceType] = amount + current;
        }
        else
        {
            resourceAmounts.Add(resourceType, amount);
        }
    }

    /// <summary>
    /// Runs a check and will return a boolean based on if the resources were successfully removed
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private bool RemoveResources(ResourceType resourceType, int amount)
    {
        //Runs a check to ensure there are enough resources, then removes the resources if theres enough
        if (CheckEnoughResources(resourceType, amount))
        {
            resourceAmounts[resourceType] -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// The check for if the controller has enough resources
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CheckEnoughResources(ResourceType resourceType, int amount)
    {
        //Checks the dictionary to see if the team has enough resources required
        if (amount > resourceAmounts[resourceType])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: add some sort of ui that displays resources, and a function to update that
}
