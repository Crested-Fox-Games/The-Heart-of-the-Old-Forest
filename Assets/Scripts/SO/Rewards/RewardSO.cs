using UnityEngine;

[System.Serializable]
public struct RarityRewardEffect
{
    /// <summary>
    /// The rarity of the reward granted to the player
    /// </summary>
    public Rarity Rarity;

    /// <summary>
    /// The value for the rarity selected
    /// </summary>
    [Tooltip("The amount that the reward for this rarity gives")]
    public float RewardAmount;
}

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
    /// A list of the effects for each rarity
    /// </summary>
    public RarityRewardEffect[] RarityEffects;

    /// <summary>
    /// The function that handles giving the player rewards
    /// </summary>
    /// <param name="player"></param>
    public abstract void GrantReward(PlayerRef player, Rarity rarity);

    /// <summary>
    /// Selects the rarity for the reward when granting it
    /// </summary>
    public Rarity SelectRarity()
    {
        float common = 70f, uncommon = 20f, rare = 8f, mythic = 2f;

        float sum = common + uncommon + rare + mythic;

        float val = Random.Range(0, sum);

        if (val - common <= 0)
        {
            return Rarity.common;
        }
        else
        {
            val -= common;
            if (val - uncommon <= 0)
            {
                return Rarity.uncommon;
            }
            else
            {
                val -= uncommon;
                if (val - rare <= 0)
                {
                    return Rarity.rare;
                }
                else
                {
                    return Rarity.mythic;
                }
            }
        }
    }

    public float GetRewardAmount(Rarity rarity)
    {
        foreach(RarityRewardEffect effect in RarityEffects)
        {
            if(effect.Rarity == rarity)
            {
                return effect.RewardAmount;
            }
        }

        return 0f;
    }
}
