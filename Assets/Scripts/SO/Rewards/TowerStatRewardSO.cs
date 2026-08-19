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
        //Checks to ensure we are running this on the server
        if (!InstanceFinder.IsServerStarted)
            return;

        //TODO: Might need to rework this so that all towers of that type get the upgrade, not just the once out at this moment
        //      This might need to be done by having a dictionary of towers, then using a data class to store stat and amount
        //Gets all active towers and puts them into an array
        Tower[] towers = FindObjectsByType<Tower>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //Loops through all towers
        foreach (Tower tower in towers)
        {
            //If the towerSO doesnt match the current tower, skip over this one and continue the loop
            if (tower.TowerSO != towerSO)
                continue;

            //Add the upgrade to the tower
            tower.AddUpgrade(towerStat, rewardType, rewardAmount);
        }
    }
}
