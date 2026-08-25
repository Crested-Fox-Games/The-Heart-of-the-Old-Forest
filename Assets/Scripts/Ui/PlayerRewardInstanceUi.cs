using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void PopulateResource(ResourceRewardSO reward)
    {
        rewardDescription.text = $"You will gain {reward.baseAmount} of {reward.Resource}";
    }

    private void PopulateTowerUpgrades(TowerStatRewardSO reward)
    {
        if(reward.rewardType == UpgradeType.Addition)
        {
            rewardDescription.text = $"You will gain +{reward.rewardAmount} of {reward.towerStat}";
        }
        else if(reward.rewardType == UpgradeType.Multiplacation)
        {
            rewardDescription.text = $"You will gain x{reward.rewardAmount} bonus to {reward.towerStat}";
        }
    }
}
