using UnityEngine;

[CreateAssetMenu(fileName = "ResourceRewardSO", menuName = "Scriptable Objects/ResourceRewardSO")]
public class ResourceRewardSO : RewardSO
{
    /// <summary>
    /// The resource granted by the reward system
    /// </summary>
    public ResourceSO Resource;

    //TODO: Make amount scale if we decide to based on nights cleared (Maybe on blight cleared too?)
    /// <summary>
    /// The base amount of resources granted by the reward system
    /// </summary>
    public int baseAmount;

    public override void GrantReward(PlayerRef player)
    {
        //Grant the player resources when this reward is selected
        player.playerInteraction.AcquireResources(Resource.ResourceType, baseAmount);
    }
}
