using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerRewardInstanceUi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI rewardName, rewardDescription;

    [SerializeField]
    private Image background;

    /// <summary>
    /// TODO: Add this in once we've added the sprites
    /// </summary>
    [SerializeField]
    private Image rewardIcon;

    public void Populate(RewardSO reward, Rarity rarity)
    {
        rewardName.text = reward.RewardName;

        if (reward is ResourceRewardSO resourceReward)
        {
            PopulateResource(resourceReward, rarity);
        }
        else if (reward is TowerStatRewardSO statReward)
        {
            PopulateTowerUpgrades(statReward, rarity);
        }
        else if (reward is PlayerStatRewardSO playerReward)
        {
            PopulatePlayerUpgrades(playerReward, rarity);
        }
        else if (reward is AbilityStatRewardSO abilityReward)
        {
            PopulateAbilityUpgrades(abilityReward, rarity);
        }

        background.color = GetRarityColour(rarity);
    }

    /// <summary>
    /// Gets color of the rarity
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    private Color GetRarityColour(Rarity rarity)
    {
        Color color = rarity switch
        {
            Rarity.common => Color.white,
            Rarity.uncommon => Color.green,
            Rarity.rare => Color.blue,
            Rarity.mythic => Color.gold,
            _ => Color.white
        };

        color.a = 0.5f;

        return color;
    }

    private void PopulateResource(ResourceRewardSO reward, Rarity rarity)
    {
        rewardDescription.text = $"You will gain {reward.GetRewardAmount(rarity)} {reward.Resource.ResourceName}";
    }

    private void PopulateTowerUpgrades(TowerStatRewardSO reward, Rarity rarity)
    {
        if(reward.rewardType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.GetRewardAmount(rarity)} {reward.towerStat} for this tower";
        }
        else if(reward.rewardType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.GetRewardAmount(rarity)} bonus {reward.towerStat} for this tower";
        }
    }

    private void PopulatePlayerUpgrades(PlayerStatRewardSO reward, Rarity rarity)
    {
        if (reward.upgradeType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.GetRewardAmount(rarity)} {reward.playerStat}";
        }
        else if (reward.upgradeType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.GetRewardAmount(rarity)} bonus {reward.playerStat}";
        }
    }

    private void PopulateAbilityUpgrades(AbilityStatRewardSO reward, Rarity rarity)
    {
        if (reward.upgradeType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.GetRewardAmount(rarity)} {reward.abilityStat} for this ability";
        }
        else if (reward.upgradeType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.GetRewardAmount(rarity)} bonus {reward.abilityStat} for this ability";
        }
    }
}
