using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStatReward", menuName = "Rewards/Ability Stat Reward")]
public class AbilityStatRewardSO : RewardSO
{
    /// <summary>
    /// The ability that the reward system will upgrade
    /// </summary>
    [Header("--- Reward Info ---")]
    public AbilitySO abilitySO;

    /// <summary>
    /// The stat granted by the reward system
    /// </summary>
    public AbilityStats abilityStat;

    /// <summary>
    /// Whether the upgrade is additive or multiplicative
    /// </summary>
    public UpgradeType upgradeType;

    public override void GrantReward(PlayerRef player)
    {
        base.GrantReward(player);

        player.playerAbilities.AddAbilityUpgrade(abilitySO, abilityStat, upgradeType, GetRewardAmount(selectedRarity));
    }
}
