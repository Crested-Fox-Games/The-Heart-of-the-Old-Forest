using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlightManager : NetworkBehaviour
{
    //TODO: Get nodes updating the forward nodes when they've been cleared, if theres one missing in between need to caluclate that

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
        //Adds all the initial starting points to the forward nodes
        currentForwardNodes.AddRange(startingPoints);

        heartCrystal = FindFirstObjectByType<HeartCrystal>();

        StartSpawnLoop();
    }

    private void StartSpawnLoop()
    {
        //TODO: make a coroutine for storing this
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while(true)
        {
            Debug.Log("We hit another loop");
            JumpNode();

            yield return new WaitForSeconds(5f);

        }
    }

    /// <summary>
    /// Selects the point at which the blight will jump forward from
    /// </summary>
    /// <returns></returns>
    private Transform SelectJumpPoint()
    {
        int index = Random.Range(0, currentForwardNodes.Count);

        return currentForwardNodes[index];
    }

    private void JumpNode()
    {
        Transform jumpNode = SelectJumpPoint();

        //Get the direction from the selected node to the crystal
        Vector3 direction = (heartCrystal.transform.position - jumpNode.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation(direction);
        //Reset Y so it doesnt go up
        direction.y = 0;
        //TODO: Give it a cone range it can spawn in, maybe min and max dist too

        //Gets the position the node will spawn at
        Vector3 targetPos = jumpNode.position + direction * 10;

        BlightNode currentNode = Instantiate(blightPrefab, targetPos, lookDirection);

        Debug.Log(currentNode);

        //Spawns the object on the server
        Spawn(currentNode.gameObject);

        currentForwardNodes.Remove(jumpNode);

        currentForwardNodes.Add(currentNode.transform);
    }
}
