using UnityEngine;

public abstract class RewardSO : ScriptableObject
{
    /// <summary>
    /// The name of the reward granted to the player
    /// </summary>
    [Header("--- Ui Info ---")]
    public string RewardName;

    /// <summary>
    /// The icon for the reward granted to the player
    /// </summary>
    public Sprite RewardIcon;

    /// <summary>
    /// The function that handles giving the player rewards
    /// </summary>
    /// <param name="player"></param>
    public abstract void GrantReward(PlayerRef player);
}
