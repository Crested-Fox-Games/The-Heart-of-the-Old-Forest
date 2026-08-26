using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlightNode : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private GameObject blightModel;

    private Transform nextBlightNode, previousBlightNode;

    private List<ResourceNode> blightedNodes = new List<ResourceNode>();

    [SerializeField]
    private float interactTime = 3f;

    public float InteractTime => interactTime;

    /// <summary>
    /// The factor that the rarity of the blight will scale it by
    /// </summary>
    [SerializeField]
    private float blightUncommonMult = 1.5f, blightRareMult = 2f, blightMythicMult = 3f;

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

        SetRarityScales(rarity);
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

        //Tells the blight manager that a blight node has been cleared
        BlightManager.Instance.BlightCleared();

        //Update the enemy logic
        UpdateEnemies();

        //Despawn the blight node
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
    /// Scales the blight enemies health and damage based on its rarity
    /// </summary>
    /// <param name="rarity"></param>
    private void SetRarityScales(BlightRarity rarity)
    {
        //Update the scale and buff the blight based on the rarity multiplier
        switch(rarity)
        {
            case BlightRarity.uncommon:
                blightModel.transform.localScale = Vector3.one * blightUncommonMult;
                BuffBlight(blightUncommonMult, blightUncommonMult);
                break;
            case BlightRarity.rare:
                blightModel.transform.localScale = Vector3.one * blightRareMult;
                BuffBlight(blightRareMult, blightRareMult);
                break;
            case BlightRarity.mythic:
                blightModel.transform.localScale = Vector3.one * blightMythicMult;
                BuffBlight(blightMythicMult, blightMythicMult);
                break;
        }

        //Fix the position of the node
        Renderer modelRender = blightModel.GetComponent<Renderer>();
        blightModel.transform.position = new Vector3(modelRender.transform.position.x, blightModel.transform.localScale.y, modelRender.transform.position.z);

        //Tell the reveal controller to update the size for revealing
        blightModel.GetComponent<RevealController>().UpdateRender();
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

    /// <summary>
    /// Overload of the buff blight for passing in values
    /// </summary>
    /// <param name="healthMult"></param>
    /// <param name="damageMult"></param>
    private void BuffBlight(float healthMult, float damageMult)
    {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.TryGetComponent<Enemy>(out Enemy enemy))
            {
                //Scales up the blight creatures stats whenever another node is cleared
                enemy.ScaleBlightStats(healthMult, damageMult);
            }
        }
    }

    /// <summary>
    /// Update the enemies so that theyre no longer children of the blight node
    /// and update their logic
    /// </summary>
    private void UpdateEnemies()
    {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.TryGetComponent<Enemy>(out Enemy enemy))
            {
                //Update the parent of the child to be the enemy spawner
                child.parent = FindFirstObjectByType<EnemySpawner>().gameObject.transform;

                //MARCUS TODO: Tell the enemy to update its logic to be a wave enemy or whatever we decide to do
            }
        }
    }
}
