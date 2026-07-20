using FishNet.Object;
using UnityEngine;

public class TempTowerPlacement : NetworkBehaviour, IInteractable
{

    public bool CanInteract(NetworkObject player)
    {
        //TODO: Check if tower placement UI is already open, also check if player is close enough
        return true;
    }

    public void Interact(NetworkObject player)
    {
        //Brings up the tower placement UI
        UiManager.Instance.ShowTowerPlacementUi();
    }

    /// <summary>
    /// The function called when the player selects a tower from the tower placement UI
    /// </summary>
    /// <param name="towerSO"></param>
    public void PlaceTower(TowerSO towerSO)
    {

    }
}
