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

    /// <summary>
    /// The sprites used for the background of the reward ui
    /// </summary>
    [SerializeField]
    private Sprite commonBg, uncommonBg, rareBg, mythicBg;

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

        background.sprite = GetRarityColour(rarity);
    }

    /// <summary>
    /// Gets sprite of the rarity
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    private Sprite GetRarityColour(Rarity rarity)
    {
        Sprite sprite = rarity switch
        {
            Rarity.common => commonBg,
            Rarity.uncommon => uncommonBg,
            Rarity.rare => rareBg,
            Rarity.mythic => mythicBg,
            _ => commonBg
        };

        return sprite;
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
