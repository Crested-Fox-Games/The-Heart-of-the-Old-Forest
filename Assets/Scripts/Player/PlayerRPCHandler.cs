using FishNet.Connection;
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

    /// <summary>
    /// Tells the server to place the tower in the slot
    /// </summary>
    /// <param name="currentTowerSlot"></param>
    /// <param name="towerName"></param>
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

    /// <summary>
    /// Tells the server which reward the player selected
    /// </summary>
    /// <param name="rewardId"></param>
    /// <param name="conn"></param>
    [ServerRpc]
    public void SelectNightlyReward(int rewardId, NetworkConnection conn = null)
    {
        RewardManager.Instance.SelectReward(rewardId, conn);
    }

    /// <summary>
    /// The server tells the specific client what its nightly rewards are
    /// </summary>
    /// <param name="rewardIds"></param>
    [TargetRpc]
    public void ShowNightlyRewards(NetworkConnection conn, int[] rewardIds)
    {
        //TODO: Call the ui manager and get it to show the rewards
    }
}
