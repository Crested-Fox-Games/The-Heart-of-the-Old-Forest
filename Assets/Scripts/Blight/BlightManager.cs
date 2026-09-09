using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The rarity levels for blight nodes
/// </summary>
public enum Rarity
{
    common,
    uncommon,
    rare,
    mythic,
}

public class BlightManager : NetworkBehaviour
{
    public static BlightManager Instance { get; private set; }

    /// <summary>
    /// The starting positions that the nodes can spawn from
    /// </summary>
    [SerializeField]
    private List<Transform> startingPoints = new List<Transform>();

    /// <summary>
    /// The current forwardmost node positions
    /// </summary>
    private List<Transform> currentForwardNodes = new List<Transform>();

    private HeartCrystal heartCrystal;

    [SerializeField]
    private BlightNode blightPrefab;

    /// <summary>
    /// The range on either side of the direction for the node to spawn at. (45 means a total of 90 degree cone)
    /// </summary>
    [SerializeField]
    private float spawnConeRange = 45f;

    /// <summary>
    /// The distance range the nodes can move forward towards the node by
    /// </summary>
    [SerializeField]
    private float minDistance = 10f, maxDistance = 20f;

    /// <summary>
    /// The time range that the nodes will spawn at
    /// </summary>
    [SerializeField]
    private float minTime = 300f, maxTime = 420f;

    /// <summary>
    /// Tracks the amount of blight nodes that have been cleared
    /// </summary>
    public float blightNodesCleared { get; private set; }

    /// <summary>
    /// The amount of blight that needs to be cleared in order to buff them
    /// </summary>
    [SerializeField]
    private float blightBossRequirement;

    /// <summary>
    /// The event that triggers when blight nodes are cleared and buffs the rest
    /// </summary>
    public event Action BlightNodesBuffed;

    /// <summary>
    /// The event that triggers when enough blight nodes are cleared and spawns a boss enemy in the next night
    /// </summary>
    public event Action BlightBossSpawn;

    public event Action<NetworkObject> BlightNodeSpawned;

    /// <summary>
    /// Setting up the pools that the blight nodes can spawn with
    /// </summary>
    private Dictionary<Rarity, float> Stage1Pool = new()
    {
        [Rarity.common] = 95f,
        [Rarity.uncommon] = 5f
    };

    private Dictionary<Rarity, float> Stage2Pool = new()
    {
        [Rarity.common] = 75f,
        [Rarity.uncommon] = 20f,
        [Rarity.rare] = 5f
    };

    private Dictionary<Rarity, float> Stage3Pool = new()
    {
        [Rarity.common] = 35f,
        [Rarity.uncommon] = 40f,
        [Rarity.rare] = 20f,
        [Rarity.mythic] = 5f
    };

    private Dictionary<Rarity, float> Stage4Pool = new()
    {
        [Rarity.common] = 10f,
        [Rarity.uncommon] = 30f,
        [Rarity.rare] = 40f,
        [Rarity.mythic] = 20f
    };

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

    public override void OnStartServer()
    {
        base.OnStartServer();

        //Adds all the initial starting points to the forward nodes
        currentForwardNodes.AddRange(startingPoints);

        StartCoroutine(Initialize());
    }

    private void Start()
    {
        TimeCycleManager.Instance.OnNightEnd += BuffBlightNodes;
    }

    /// <summary>
    /// Coroutine to ensure heart crystal is set before starting blight
    /// </summary>
    /// <returns></returns>
    private IEnumerator Initialize()
    {
        //Done in an ienumerator to ensure we find it properly
        while(heartCrystal == null)
        {
            heartCrystal = FindFirstObjectByType<HeartCrystal>();

            yield return null;
        }

        StartSpawnLoop();
    }

    private void StartSpawnLoop()
    {
        //TODO: make a coroutine for storing this
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// The loop that spawns in the Blight nodes
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnLoop()
    {
        //This gives a x second window before any nodes are spawned, allowing for systems to start up 
        yield return new WaitForSeconds(5f);

        while(true)
        {
            //Debug.Log("We hit another loop");
            JumpNode();

            float timer = Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(timer);

        }
    }

    /// <summary>
    /// Selects the point at which the blight will jump forward from
    /// </summary>
    /// <returns></returns>
    private Transform SelectJumpPoint()
    {
        int index = Random.Range(0, currentForwardNodes.Count);

        Debug.Log($"Blight node to spawn from selected as {currentForwardNodes[index].name} at position {currentForwardNodes[index].transform.position}");

        return currentForwardNodes[index];
    }

    /// <summary>
    /// Jumps the node forward towards the heart crystal
    /// </summary>
    private void JumpNode()
    {
        Transform jumpNode = SelectJumpPoint();

        //Get the direction from the selected node to the crystal
        Vector3 direction = heartCrystal.transform.position - jumpNode.position;

        //Reset Y so it doesnt go up
        direction.y = 0;
        direction.Normalize();

        //Give it a cone range it can spawn in
        float rangeOffset = Random.Range(-spawnConeRange, spawnConeRange);

        //Min and max dist 
        float distOffset = Random.Range(minDistance, maxDistance);

        Vector3 finalDirection = Quaternion.Euler(0f, rangeOffset, 0f) * direction;

        //Gets the position the node will spawn at
        Vector3 targetPos = jumpNode.position + finalDirection * distOffset;

        Quaternion lookDirection = Quaternion.LookRotation(finalDirection);

        BlightNode currentNode = Instantiate(blightPrefab, targetPos, lookDirection);

        //Spawns the object on the server
        Spawn(currentNode.gameObject);

        //Activates the event for blight nodes being spawned
        //This tells the enemy spawner to spawn a cluster of blight enemies
        BlightNodeSpawned?.Invoke(currentNode);


        currentNode.Initialize(jumpNode, GetBlightRarity());

        if(jumpNode.TryGetComponent<BlightNode>(out BlightNode node))
        {
            node.SetNextNode(currentNode.transform);
        }

        currentForwardNodes.Remove(jumpNode);

        currentForwardNodes.Add(currentNode.transform);
    }

    /// <summary>
    /// This function allows the spawned nodes to update the jump nodes
    /// </summary>
    public void UpdateForwardNodesFromNode(Transform nodeToRemove, Transform nodeToAdd)
    {
        Debug.Log($"We are removing {nodeToRemove.name}");
        currentForwardNodes.Remove(nodeToRemove);

        currentForwardNodes.Add(nodeToAdd);

        Debug.Log($"We just added {nodeToAdd.name}");
    }

    /// <summary>
    /// Handles adding to the amount of blight nodes cleared
    /// </summary>
    public void BlightCleared()
    {
        blightNodesCleared++;

        //Checks to see if the remainder of blight cleared divided by blight buff is 0, then does logic
        if (blightNodesCleared % blightBossRequirement == 0)
        {
            //Tells the enemy spawner to spawn a boss on the next night
            FindFirstObjectByType<EnemySpawner>().bossToSpawn = true;
        }
    }

    private void BuffBlightNodes()
    {
        //Buffs the existing blight nodes
        BlightNodesBuffed?.Invoke();
    }

    /// <summary>
    /// Gets the rarity for the blight node we are spawning
    /// </summary>
    /// <returns></returns>
    private Rarity GetBlightRarity()
    {
        //Gets the current rarity pool for the rarity weights
        Dictionary<Rarity, float> currentPool = GetCurrentRarityPool();

        //Calculates total weights in pool and selects a random number in that range
        float totalWeight = currentPool.Values.Sum();
        float selected = Random.Range(0, totalWeight);

        //Loops through the rarities
        foreach(var rarity in currentPool)
        {
            //Takes away the rarity value from the selected value
            selected -= rarity.Value;

            //Checks to see if the selected value is at or below 0 and returns that rarity if its the case
            if(selected <= 0)
            {
                return rarity.Key;
            }
        }

        return currentPool.First().Key;
    }

    /// <summary>
    /// Basic implementation for getting rarity pool based on days completed
    /// </summary>
    /// <returns></returns>
    private Dictionary<Rarity, float> GetCurrentRarityPool()
    {
        //TODO: Probably want to design this better
        if (TimeCycleManager.Instance.CurrentDay < 2)
        {
            return Stage1Pool;
        }
        else if (TimeCycleManager.Instance.CurrentDay < 4)
        {
            return Stage2Pool;
        }
        else if (TimeCycleManager.Instance.CurrentDay < 6)
        {
            return Stage3Pool;
        }

        return Stage4Pool;
    }
}
