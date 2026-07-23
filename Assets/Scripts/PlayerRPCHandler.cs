using FishNet.Object;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is needed as the networking is being a fuck ass. 
/// Apparently to call a server RPC it needs to come from an object owned by the player.
/// As of writing this, the player is the only thing owned by the player, thus, this shit hole script was born.
/// </summary>
public class PlayerRPCHandler : NetworkBehaviour
{
    public static PlayerRPCHandler Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [ServerRpc]
    public void CallPlaceTower(NetworkObject currentTowerSlot, string towerName)
    {
        //Loads the resources because the network isnt serializing the SO's 
        TowerSO towerSO = Resources.LoadAll<TowerSO>("ScriptableObjects")
            .FirstOrDefault(s => s.TowerName == towerName);

        if (towerSO == null)
            return;

        currentTowerSlot.GetComponent<TempTowerPlacement>().PlaceTower(towerSO);
    }
}
