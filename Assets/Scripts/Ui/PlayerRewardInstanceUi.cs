using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerRewardInstanceUi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI rewardName, rewardDescription;

    /// <summary>
    /// TODO: Add this in once we've added the sprites
    /// </summary>
    [SerializeField]
    private Image rewardIcon;

    public void Populate(RewardSO reward)
    {
        rewardName.text = reward.RewardName;

        if (reward is ResourceRewardSO resourceReward)
        {
            PopulateResource(resourceReward);
        }
        else if (reward is TowerStatRewardSO statReward)
        {
            PopulateTowerUpgrades(statReward);
        }
        else if (reward is PlayerStatRewardSO playerReward)
        {
            PopulatePlayerUpgrades(playerReward);
        }
        else if (reward is AbilityStatRewardSO abilityReward)
        {
            PopulateAbilityUpgrades(abilityReward);
        }

    }

    private void PopulateResource(ResourceRewardSO reward)
    {
        rewardDescription.text = $"You will gain {reward.baseAmount} {reward.Resource.ResourceName}";
    }

    private void PopulateTowerUpgrades(TowerStatRewardSO reward)
    {
        if(reward.rewardType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.rewardAmount} {reward.towerStat} for this tower";
        }
        else if(reward.rewardType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.rewardAmount} bonus {reward.towerStat} for this tower";
        }
    }

    private void PopulatePlayerUpgrades(PlayerStatRewardSO reward)
    {
        if (reward.upgradeType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.rewardAmount} {reward.upgradeType} for this tower";
        }
        else if (reward.upgradeType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.rewardAmount} bonus {reward.upgradeType} for this tower";
        }
    }

    private void PopulateAbilityUpgrades(AbilityStatRewardSO reward)
    {
        if (reward.upgradeType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.rewardAmount} {reward.upgradeType} for this tower";
        }
        else if (reward.upgradeType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain a x{reward.rewardAmount} bonus {reward.upgradeType} for this tower";
        }
    }
}
