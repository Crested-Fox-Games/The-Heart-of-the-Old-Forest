using System.Collections.Generic;
using UnityEngine;

public class BaseResourceController : MonoBehaviour
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
        //Player player;

        /*
         * if(other.TryGetComponent<player>(out player) != null)
         * {
         *      player.CollectResources()
         * }
         */

    }

    private void AddResources(ResourceType resourceType, int amount)
    {
        //Adds resources to the stockpile
        //NOTE: this should automatically new fields to the dictionary if they dont exist already
        resourceAmounts[resourceType] += amount;
    }

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
}
