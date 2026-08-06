using FishNet.Object;
using UnityEngine;

public class BlightNode : NetworkBehaviour, IInteractable
{
    //TODO Add clearing the nodes
    private Transform nextBlightNode, previousBlightNode;

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
        throw new System.NotImplementedException();
    }

    
    public bool CanInteract(NetworkObject player)
    {
        throw new System.NotImplementedException();
    }
}
