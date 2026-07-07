using UnityEngine;

/// <summary>
/// This interface is used to define the objects that can be interacted with by the player.
/// This is used in PlayerInteraction to display a popup for the input button that they need to press to interact with it.
/// To implement it, look at the ResourceNode class where the class is defined at the top.
/// </summary>
public interface IInteractable
{
    void Interact();
}
