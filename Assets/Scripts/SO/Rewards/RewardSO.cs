using UnityEngine;

[CreateAssetMenu(fileName = "RewardSO", menuName = "Scriptable Objects/RewardSO")]
public abstract class RewardSO : ScriptableObject
{
    public string RewardName;
    public Sprite RewardIcon;

    /// <summary>
    /// The function that handles giving the player rewards
    /// </summary>
    /// <param name="player"></param>
    public abstract void GrantReward(PlayerRef player);
}
