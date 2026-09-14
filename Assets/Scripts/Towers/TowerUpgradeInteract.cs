using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class TowerUpgradeInteract : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private float interactTime = 0.5f;

    public float InteractTime => interactTime;

    public bool CanInteract(NetworkObject player)
    {
        //TODO: Check if tower can be upgraded, also check if player is close enough
        return true;
    }

    public void Interact(NetworkObject player)
    {
        //Tells the client to do its interactions
        HandleInteraction(player.Owner);
    }

    /// <summary>
    /// Target Rpc lets the server tell only a single client to run the function
    /// That way only the client that called the server for it will be updates
    /// </summary>
    /// <param name="conn"></param>
    [TargetRpc]
    private void HandleInteraction(NetworkConnection conn)
    {
        //Brings up the tower placement UI
        UiManager.Instance.ShowTowerUpgradeUi();
        TowerPlacementUi.Instance.SetCurrentSlot(this);
    }
}
