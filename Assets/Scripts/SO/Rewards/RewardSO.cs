using UnityEngine;

public abstract class RewardSO : ScriptableObject
{
    /// <summary>
    /// The name of the reward granted to the player
    /// </summary>
    [Header("--- Ui Info ---")]
    public string RewardName;

    /// <summary>
    /// The ID of the reward SO
    /// </summary>
    public int RewardId;

    /// <summary>
    /// The weight of the reward that is selected
    /// </summary>
    public float RewardWeight;

    /// <summary>
    /// The icon for the reward granted to the player
    /// </summary>
    public Sprite RewardIcon;

    /// <summary>
    /// The rarity of the reward granted to the player
    /// </summary>
    public Rarity rarity;

    /// <summary>
    /// The function that handles giving the player rewards
    /// </summary>
    /// <param name="player"></param>
    public abstract void GrantReward(PlayerRef player);
}
