using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRewardUi : MonoBehaviour
{
    //TODO: Have 3 functions linked to each reward and have them call the playerRPCHandler
    //      and pass through the id of the selected one

    /// <summary>
    /// The options the player can pick from
    /// </summary>
    [SerializeField]
    private PlayerRewardInstanceUi option1, option2, option3;

    /// <summary>
    /// Dictionary that contains all of the rewards with their id as the key
    /// </summary>
    private Dictionary<int, RewardSO> rewards = new Dictionary<int, RewardSO>();

    private List<RewardSO> currentRewardOptions = new List<RewardSO>();

    private PlayerRef localPlayer;

    private void Start()
    {
        //Gets a dictionary of all rewardSO's 
        rewards = Resources.LoadAll<RewardSO>("Rewards").ToDictionary(reward => reward.RewardId);
        Debug.Log($"Rewards count {rewards.Count}");

        //Closes it straight away
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets the 3 rewards sent in through the int array 
    /// </summary>
    /// <param name="rewardIds"></param>
    public void ReceiveRewardData(int[] rewardIds, PlayerRef player)
    {
        currentRewardOptions.Clear();

        localPlayer = player;

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"Reward {i}: Reward id{rewardIds[i]} : Corresponding reward {rewards[i].RewardName}"  );
            currentRewardOptions.Add(rewards[rewardIds[i]]);
        }

        PopulateUi();
    }

    /// <summary>
    /// Populates 3 rewards screens with the info from their SO
    /// </summary>
    /// <param name="rewardList"></param>
    private void PopulateUi()
    {
        option1.Populate(currentRewardOptions[0]);
        option2.Populate(currentRewardOptions[1]);
        option3.Populate(currentRewardOptions[2]);
    }

    public void SelectOption1()
    {
        localPlayer.playerRPCHandler.SelectNightlyReward(currentRewardOptions[0].RewardId);
        UiManager.Instance.CloseRewardScreen();
    }

    public void SelectOption2()
    {
        localPlayer.playerRPCHandler.SelectNightlyReward(currentRewardOptions[1].RewardId);
        UiManager.Instance.CloseRewardScreen();
    }

    public void SelectOption3()
    {
        localPlayer.playerRPCHandler.SelectNightlyReward(currentRewardOptions[2].RewardId);
        UiManager.Instance.CloseRewardScreen();
    }
}
