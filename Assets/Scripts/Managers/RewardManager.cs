using FishNet.Object;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : NetworkBehaviour
{
    public static RewardManager Instance { get; private set; }

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

    public List<RewardSO> GenerateRewards(int amount)
    {
        //TODO: Select rewards from the pool
        return null;
    }

    [ServerRpc]
    public void SelectReward(int rewardIndex)
    {

    }
}
