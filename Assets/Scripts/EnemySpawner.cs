using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// This data class is responsible for what is needed in a cluster of enemies
/// </summary>
[System.Serializable]
public class EnemyCluster
{
    /// <summary>
    /// The minimum amount of points needed in a cluster
    /// </summary>
    public float minSpawnValue;

    //TODO: figure out a way to determine the radius of the clusters spawns
    /// <summary>
    /// The radius that the enemies in the cluster spawn in
    /// </summary>
    public float radius = 5f;

    /// <summary>
    /// The list of enemies in the cluster
    /// </summary>
    public List<GameObject> enemies = new();
}

public class EnemySpawner : MonoBehaviour
{
    //TODO: Some sort of way to change spawn patterns (timer between spawns, amount of enemies at once, enemies stats)
    //based on conditions like bosses killed, amount of days passed, total player count
    
    //TODO: Dictionary of all enemies in the game that can be spawned at night

    /// <summary>
    /// The list of currently unlocked enemies that can be spawned.
    /// </summary>
    [SerializeField]
    private List<GameObject> unlockedEnemies;

    [SerializeField]
    private TimeManager timeManager;

    /// <summary>
    /// The gameobject for the heart crystal, so that this can be passed to enemies that spawn
    /// </summary>
    [SerializeField]
    private GameObject HeartCrystal;

    /// <summary>
    /// The coroutine used to ensure that the enemy spawns stop at the end of the night cycle
    /// </summary>
    private Coroutine spawnCoroutine;

    /// <summary>
    /// The base time between enemies spawning, might be modified later based on different conditions
    /// </summary>
    [SerializeField]
    private float baseSpawnInterval = 5f; 

    /// <summary>
    /// The current amount of enemies that have been spawned
    /// </summary>
    private int spawnCount = 0;

    /// <summary>
    /// The max amount of enemies that can be spawned at once
    /// </summary>
    private int maxSpawnCount = 100;

    private void Start()
    {
        //This subscribes the functions to the time manager so that when the events fire, these functions will trigger
        timeManager.OnNightStart += NightStarted;
        timeManager.OnNightEnd += NightEnded;
    }

    /// <summary>
    /// Handles what happens at the start of the night cycle
    /// </summary>
    private void NightStarted()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Handles what happens at the end of the night cycle/start of the day cycle
    /// </summary>
    private void NightEnded()
    {
        StopCoroutine(spawnCoroutine);

        //TODO: decide if all the enemies die once day hits, to encourage day exploration instead of wasting time killing leftover enemies
    }

    /// <summary>
    /// The coroutine that handles the timing of enemy spawns
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if(spawnCount < maxSpawnCount)
            {
                SpawnCluster();
            }
            
            yield return new WaitForSeconds(baseSpawnInterval);
        }
    }

    /// <summary>
    /// Used for spawning a cluster of enemies
    /// </summary>
    private void SpawnCluster()
    {
        //Creates a cluser of random enemies
        EnemyCluster cluster = GetRandomCluster();

        Debug.Log("Spawning cluster with " + cluster.enemies.Count + " enemies and a minimum spawn value of: " + cluster.minSpawnValue);

        foreach (GameObject enemy in cluster.enemies)
        {
            SpawnEnemy(enemy, cluster.radius);
        }
    }

    /// <summary>
    /// Gets a random cluster of enemies based on the spawn values
    /// </summary>
    private EnemyCluster GetRandomCluster()
    {
        //Creates a new cluster data type to be populated
        EnemyCluster newCluster = new EnemyCluster();

        float totalWeight = 0;

        //TODO: decide how we're actually going to do this, probably some game manager that handles scaling
        newCluster.minSpawnValue = Random.Range(50, 100);

        //Probably redundant but just to be safe
        newCluster.enemies.Clear();

        //Loop keeps adding enemies to the cluster until the total weight is above the minimum
        while (totalWeight < newCluster.minSpawnValue)
        {
            //TODO: change this to be based on a spawn weight system
            //(Also need to figure out how to differentiate name with the weight for how much they cost)
            //(Unless we make them the same thing)
            int randomIndex = Random.Range(0, unlockedEnemies.Count);

            //The weight of the current enemy
            float enemyWeight = unlockedEnemies[randomIndex].GetComponent<Enemy>().EnemySO.EnemySpawnWeight;

            if(enemyWeight <=0)
            {
                Debug.LogWarning("Enemy " + unlockedEnemies[randomIndex].name + " has a spawn weight of 0 or less, and will not be added to clusters.");

                break;
            }

            totalWeight += enemyWeight;

            newCluster.enemies.Add(unlockedEnemies[randomIndex]);
        }

        return newCluster;
    }

    /// <summary>
    /// Used for the spawning of the individual enemies
    /// </summary>
    private void SpawnEnemy(GameObject enemyPrefab, float spawnRadius)
    {
        //Used for updating values in the script once we spawn the enemy
        Enemy currentEnemy;

        //Get enemy spawn position
        Vector3 pos = GetSpawnPosition(spawnRadius);

        //Spawn enemy (uses get enemy height halved due to pivot point being in middle, might need to change if assets are different)
        currentEnemy = Instantiate(enemyPrefab, pos + GetEnemyHeightHalved(enemyPrefab), Quaternion.identity).GetComponent<Enemy>();

        Debug.Log("Spawned enemy: " + currentEnemy.name + " at position: " + pos);

        //Increment spawn count
        spawnCount++;

        //Update its parent object to the spawner
        currentEnemy.transform.parent = this.transform;

        //Set heart crystal as target for enemy
        currentEnemy.SetHeartCrystal(HeartCrystal);

        //Subscribe to enemy death event (Anonymous lambda function used to subscribe to the event)
        currentEnemy.onEnemyKilled += () => spawnCount--;
    }

    private Vector3 GetSpawnPosition(float spawnRadius)
    {
        //spawn enemies in clusters in NEWS directions, at a certain distance away.
        int direction = Random.Range(0, 4);

        //TODO: change this later
        float distance = 25f;

        Vector3 cardinalPos;

        //Get the position based on the cardinal directions
        switch (direction)
        {
            case 0: //North
                cardinalPos = new Vector3(0, 0, distance);
                break;
            case 1: //East
                cardinalPos = new Vector3(distance, 0, 0);
                break;
            case 2: //South
                cardinalPos = new Vector3(0, 0, -distance);
                break;
            case 3: //West
                cardinalPos = new Vector3(-distance, 0, 0);
                break;
            default:
                return Vector3.zero;
        }

        //This offsets the spawn pos to a random point in the spawn radius
        //TODO: Will need to check to ensure this spawn point is not inside any other enemies or obstacles
        Vector2 offset = Random.insideUnitCircle * spawnRadius;

        //TODO: Need to use something like raycast to get ground height for spawn pos

        //Uses the cardinal position and offset to get the spawn position based on the heart crystal's position
        return HeartCrystal.transform.position + cardinalPos + new Vector3(offset.x, 0, offset.y);
    }

    /// <summary>
    /// This is used to ensure that the enemy spawns at the correct height, as they spawn from center of prefab
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private Vector3 GetEnemyHeightHalved(GameObject enemy)
    {
        return new Vector3(0, enemy.GetComponent<Renderer>().bounds.size.y / 2f, 0);
    }

    //TODO: create a function that unlocks enemies based on conditions, probably have subscriptions to boss death events for example
    //This might need to be a whole class that handles progression
}
