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

    /// <summary>
    /// The currently spawned tower
    /// </summary>
    private GameObject spawnedTower;

    [SerializeField]
    private float interactTime = 1f;

    public float InteractTime => interactTime;

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
        TowerPlacementUi.Instance.SetCurrentSlot(this);
    }

    /// <summary>
    /// The function called when the player selects a tower from the tower placement UI
    /// </summary>
    /// <param name="towerSO"></param>
    public void PlaceTower(TowerSO towerSO)
    {
        if (BaseResourceController.Instance.RemoveResources(towerSO.RequiredResources))
        {
            spawnedTower = Instantiate(towerSO.TowerPrefab, transform.position, transform.rotation);

            ServerManager.Spawn(spawnedTower);

            DisableTowerSlot();

            spawnedTower.transform.parent = this.transform;

            //The tower can be placed so we close the ui for tower placement
            UiManager.Instance.HideTowerPlacementUi();
        }
    }

    [ObserversRpc]
    public void DisableTowerSlot()
    {
        towerSlotPrefab.SetActive(false);
    }

    /// <summary>
    /// This function will be for when the player destroys the tower to place a new one
    /// </summary>
    public void TowerDestroyed()
    {
        //Gets rid of the tower
        ServerManager.Despawn(spawnedTower);
        Destroy(spawnedTower);

        towerSlotPrefab.SetActive(true);
    }
}
