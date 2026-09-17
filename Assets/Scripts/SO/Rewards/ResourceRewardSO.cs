using UnityEngine;

[CreateAssetMenu(fileName = "ResourceRewardSO", menuName = "Rewards/Resource Reward")]
public class ResourceRewardSO : RewardSO
{
    /// <summary>
    /// The resource granted by the reward system
    /// </summary>
    [Header("--- Reward Info ---")]
    public ResourceSO Resource;

    public override void GrantReward(PlayerRef player)
    {
        base.GrantReward(player);

        //Grant the player resources when this reward is selected
        player.playerInteraction.AcquireResources(Resource.ResourceType, (int)GetRewardAmount(selectedRarity));
    }
}
