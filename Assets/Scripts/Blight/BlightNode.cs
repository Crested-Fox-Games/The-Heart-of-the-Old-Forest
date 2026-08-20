using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlightNode : NetworkBehaviour, IInteractable
{
    //TODO: Add spawning enemies

    private Transform nextBlightNode, previousBlightNode;

    private List<ResourceNode> blightedNodes = new List<ResourceNode>();

    [SerializeField]
    private float interactTime = 3f;

    public float InteractTime => interactTime;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ResourceNode>(out ResourceNode node))
        {
            //Corrupts nodes that are in its radius so that they cant be used
            node.UpdateNodeCorruption(true);
        }
    }

    public void Initialize(Transform previous, BlightRarity rarity)
    {
        previousBlightNode = previous;

        //Subscribe to blight buff event
        BlightManager.Instance.BlightNodesBuffed += BuffBlight;
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

                //Checks to see if there is a previous node in the chain and updates it
                if(previousBlightNode.TryGetComponent<BlightNode>(out BlightNode prevNode))
                {
                    prevNode.SetNextNode(nextBlightNode);
                }
            }
        }

        //Uncorrupt each node
        foreach (ResourceNode resource in blightedNodes)
        {
            resource.UpdateNodeCorruption(false);
        }

        BlightManager.Instance.BlightCleared();

        Despawn(this);
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

    /// <summary>
    /// Triggers when another blight node is cleared and buffs this one
    /// </summary>
    private void BuffBlight()
    {
        foreach(Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if(child.TryGetComponent<Enemy>(out Enemy enemy))
            {
                //Scales up the blight creatures stats whenever another node is cleared
                enemy.ScaleBlightStats(0.05f, 0.05f);
            }
        }
    }
}
