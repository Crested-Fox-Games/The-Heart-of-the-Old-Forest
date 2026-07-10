using FishNet.Object;
using UnityEngine;

/// <summary>
/// This interface is used to define the objects that can be interacted with by the player.
/// This is used in PlayerInteraction to display a popup for the input button that they need to press to interact with it.
/// To implement it, look at the ResourceNode class where the class is defined at the top.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// This function is used for objects that can be interacted with, it passes through the player that calls it so that the 
    /// object can validate the interaction and perform any required actions on that player
    /// </summary>
    /// <param name="player"></param>
    void Interact(NetworkObject player);

    /// <summary>
    /// This function is used to validate whether the player can interact with the object
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool CanInteract(NetworkObject player);
}
