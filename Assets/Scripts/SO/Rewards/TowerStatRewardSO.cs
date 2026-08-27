using FishNet;
using NUnit.Framework;
using UnityEngine;

public enum UpgradeType
{
    Addition,
    Multiplacation
}

[CreateAssetMenu(fileName = "TowerStatRewardSO", menuName = "Rewards/Tower Stat Reward")]
public class TowerStatRewardSO : RewardSO
{
    /// <summary>
    /// Which tower stat we will be upgrading
    /// </summary>
    [Header("--- Reward Info ---")]
    public TowerStats towerStat;

    /// <summary>
    /// The scriptable object of the tower that can be upgraded
    /// </summary>
    public TowerSO towerSO;

    /// <summary>
    /// This is whether we are doing additive upgrades or multiplicative upgrades
    /// </summary>
    public UpgradeType rewardType;

    /// <summary>
    /// The amount we are adding or multiplying by
    /// </summary>
    public float rewardAmount;

    public override void GrantReward(PlayerRef player)
    {
        TowerManager.Instance.AddGlobalUpgrade(towerSO ,towerStat, rewardType, rewardAmount);
    }
}
