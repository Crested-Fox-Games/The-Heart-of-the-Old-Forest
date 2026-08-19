using FishNet.Object;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : NetworkBehaviour
{
    

    public List<RewardSO> GenerateRewards(int amount)
    {
        //TODO: Select rewards from the pool
    }

    [ServerRpc]
    public void SelectReward(int rewardIndex)
    {

    }
}
