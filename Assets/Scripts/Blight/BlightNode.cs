using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlightNode : NetworkBehaviour, IInteractable
{
    //TODO Add clearing the nodes
    private Transform nextBlightNode, previousBlightNode;

    private List<ResourceNode> blightedNodes = new List<ResourceNode>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ResourceNode>(out ResourceNode node))
        {
            //Corrupts nodes that are in its radius so that they cant be used
            node.UpdateNodeCorruption(true);
        }
    }

    public void Initialize(Transform previous)
    {
        previousBlightNode = transform;
    }

    /// <summary>
    /// Handles what happens when a node is cleared
    /// </summary>
    private void BlightCleared()
    {
        if (nextBlightNode != null)
        {
            //If there is a next node, update it to keep the chain 
            if(nextBlightNode.TryGetComponent<BlightNode>(out BlightNode node))
            {
                node.SetPreviousNode(previousBlightNode);
            }

            //Uncorrupt each node
            foreach (ResourceNode resource in blightedNodes)
            {
                resource.UpdateNodeCorruption(false);
            }

            Despawn(this);
        }
    }

    /// <summary>
    /// Updates the previous node transform
    /// </summary>
    /// <param name="previous"></param>
    public void SetPreviousNode(Transform previous)
    {
        previousBlightNode = previous;
    }

    /// <summary>
    /// Updates the next node transform
    /// </summary>
    /// <param name="next"></param>
    public void SetNextNode(Transform next)
    {
        nextBlightNode = next;
    }

    
    public void Interact(NetworkObject player)
    {
        //Handles what happens when the player clears the node
        BlightCleared();
    }

    
    public bool CanInteract(NetworkObject player)
    {
        return true;
    }
}
