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
    public static PlayerRPCHandler LocalInstance {  get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(IsOwner)
        {
            LocalInstance = this;
        }
    }

    [ServerRpc]
    public void CallPlaceTower(NetworkObject currentTowerSlot, string towerName)
    {
        //NOTE: If the folder structure changes and this isnt changed, it will break
        //Loads the resources because the network isnt serializing the SO's 
        TowerSO towerSO = Resources.LoadAll<TowerSO>("ScriptableObjects")
            .FirstOrDefault(s => s.TowerName == towerName);

        if (towerSO == null)
            return;

        currentTowerSlot.GetComponent<TempTowerPlacement>().PlaceTower(towerSO);
    }

    [ServerRpc]
    public void SelectNightlyReward(int rewardId)
    {
        RewardManager.Instance.SelectReward(rewardId, this)
    }
}
