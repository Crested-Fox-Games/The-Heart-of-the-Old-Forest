using UnityEngine;

[CreateAssetMenu(fileName = "ResourceRewardSO", menuName = "Scriptable Objects/ResourceRewardSO")]
public class ResourceRewardSO : RewardSO
{
    //TODO: Make amount scale if we decide to based on nights cleared (Maybe on blight cleared too?)

    public ResourceSO Resource;
    public int baseAmount;

    public override void GrantReward(PlayerRef player)
    {
        //Grant the player resources when this reward is selected
    }
}
