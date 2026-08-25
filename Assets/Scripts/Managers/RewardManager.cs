using FishNet.Connection;
using FishNet.Object;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager : NetworkBehaviour
{
    public static RewardManager Instance { get; private set; }

    /// <summary>
    /// The list of all rewards
    /// </summary>
    private List<RewardSO> rewards;

    /// <summary>
    /// The rewards that are selected for the current night
    /// </summary>
    private Dictionary<PlayerRef , List<RewardSO>> selectedNightlyRewards = new Dictionary<PlayerRef, List<RewardSO>>();

    /// <summary>
    /// Dictionary of rewards, used for selecting rewards quickly
    /// </summary>
    private Dictionary<int, RewardSO> rewardsById;

    private List<PlayerRef> players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Initialize();

        TimeCycleManager.Instance.OnNightEnd += GenerateRewards;
    }

    /// <summary>
    /// Sets up everything at start
    /// </summary>
    private void Initialize()
    {
        //Gets all the players in the game
        players = FindObjectsByType<PlayerRef>(FindObjectsSortMode.InstanceID).ToList();

        //Populates the reward list with all RewardSO's in the Resources/Rewards Folder
        rewards = Resources.LoadAll<RewardSO>("Rewards").OrderBy(reward => reward.RewardId).ToList();

        //Turns the list into a dictionary with rewardId as the key
        rewardsById = rewards.ToDictionary(reward => reward.RewardId);
    }

    public void GenerateRewards()
    {
        //Loops through for each player
        foreach (PlayerRef player in players)
        {
            selectedNightlyRewards[player] = new List<RewardSO>();

            //Select rewards from the pool
            for (int i = 0; i < 3; i++)
            {
                selectedNightlyRewards[player].Add(GetRandomReward(player));
            }
        }

        StartCoroutine(SendRewardsToPlayers());
    }

    /// <summary>
    /// Send the rewards to each of the players so that their ui can show it
    /// </summary>
    /// <returns></returns>
    private IEnumerator SendRewardsToPlayers()
    {
        //Wait for x seconds before sending the rewards to players
        //TODO: Decide if we want this or the players have to press a button to bring up the reward screen
        //TODO: If press a button, decide how we handle players not clicking it before the next night, do we select a random one for them?
        yield return new WaitForSeconds(1f);

        foreach (PlayerRef player in players)
        {
            //Get the rewards list for the current player
            List<RewardSO> rewards = selectedNightlyRewards[player];

            //Get the reward ids of the SO's
            int[] rewardIds = rewards.Select(reward => reward.RewardId).ToArray();

            PlayerRPCHandler playerRPCHandler = player.playerRPCHandler;

            playerRPCHandler.ShowNightlyRewards(playerRPCHandler.Owner, rewardIds);
        }
    }

    /// <summary>
    /// This gets 1 random reward that is currently not in the selected nightly reward pool
    /// </summary>
    /// <returns></returns>
    private RewardSO GetRandomReward(PlayerRef player)
    {
        //Creates a list of rewards that dont exist in selected nightly rewards
        List<RewardSO> availableRewards = rewards.Where(reward => !selectedNightlyRewards[player].Contains(reward)).ToList();

        if(availableRewards.Count ==0)
        {
            return null;
        }

        //Get the sum of all the reward weights
        float totalWeight = availableRewards.Sum(reward => reward.RewardWeight);

        //Select a random number
        float selected = Random.Range(0, totalWeight);

        //Loop through the rewards to find the one at the selected index
        foreach (RewardSO reward in availableRewards)
        {
            //Remove the weight of the current reward from the selected value
            selected -= reward.RewardWeight;

            //Reward can be selected
            if (selected <= 0f)
            {
                return reward;
            }
        }

        //This returns the last reward in the list
        return availableRewards[^1];
    }

    
    public void SelectReward(int rewardId, NetworkConnection playerConn)
    {
        if (playerConn == null || playerConn.FirstObject == null)
        {
            Debug.LogWarning($"Either the players network connection is broken, or the its passing through an invalid object.");
            return;
        }

        //Get the player ref from the connection
        PlayerRef player = playerConn.FirstObject.GetComponent<PlayerRef>();

        if (player == null)
        {
            Debug.LogWarning($"Could not find player ref for {playerConn} when selecting reward in Reward Manager.");
            return;
        }

        //Gets the reward from the dictionary
        RewardSO selectedReward = rewardsById[rewardId];

        //Grants the reward for the player
        selectedReward.GrantReward(player);

        //Clears the rewards
        selectedNightlyRewards.Remove(player);
    }
}
