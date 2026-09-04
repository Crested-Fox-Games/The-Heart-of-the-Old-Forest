using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatRewardSO", menuName = "Rewards/Player Stat Reward")]
public class PlayerStatRewardSO : RewardSO
{
    /// <summary>
    /// The stat granted by the reward system
    /// </summary>
    [Header("--- Reward Info ---")]
    [Tooltip("Base speed is 0.2f")]
    public PlayerStats playerStat;

    /// <summary>
    /// Whether the upgrade is additive or multiplicative
    /// </summary>
    public UpgradeType upgradeType;

    /// <summary>
    /// The amount of the stat granted by the reward system
    /// </summary>
    public float rewardAmount;

    public override void GrantReward(PlayerRef player)
    {
        player.playerStatus.AddUpgrade(playerStat, upgradeType, rewardAmount);
    }
}
