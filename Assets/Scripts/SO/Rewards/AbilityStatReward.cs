using UnityEngine;

/// <summary>
/// The stats of the ability that can be upgraded
/// </summary>
public enum AbilityStat
{
    Damage,
    Cooldown,
    Range,
    Speed,
    Duration
}

[CreateAssetMenu(fileName = "AbilityStatReward", menuName = "Rewards/Ability Stat Reward")]
public class AbilityStatReward : RewardSO
{
    /// <summary>
    /// The stat granted by the reward system
    /// </summary>
    [Header("--- Reward Info ---")]
    public AbilityStat abilityStat;

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
        throw new System.NotImplementedException();
    }
}
