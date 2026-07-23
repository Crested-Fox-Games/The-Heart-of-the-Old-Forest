using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class TempTowerPlacement : NetworkBehaviour, IInteractable
{
    /// <summary>
    /// The prefab that is shown for the broken/placehold tower
    /// </summary>
    [SerializeField]
    private GameObject towerSlotPrefab;

    public bool CanInteract(NetworkObject player)
    {
        //TODO: Check if tower placement UI is already open, also check if player is close enough
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
        UiManager.Instance.ShowTowerPlacementUi();
        TowerPlacementUi.Instance.SetCurrentSlot(gameObject);
    }

    /// <summary>
    /// The function called when the player selects a tower from the tower placement UI
    /// </summary>
    /// <param name="towerSO"></param>
    public void PlaceTower(TowerSO towerSO)
    {
        towerSlotPrefab.SetActive(false);

        GameObject spawnedTower = Instantiate(towerSO.TowerPrefab, transform.position, transform.rotation);

        spawnedTower.transform.parent = this.transform;
    }
}
