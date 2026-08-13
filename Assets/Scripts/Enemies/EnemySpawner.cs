using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<EnemySO> enemies = new();

    /// <summary>
    /// The direction that the cluster will spawn in
    /// </summary>
    public int direction = 0;
}

public class EnemySpawner : NetworkBehaviour
{
    //TODO: Some sort of way to change spawn patterns (timer between spawns, amount of enemies at once, enemies stats)
    //based on conditions like bosses killed, amount of days passed, total player count
    
    //TODO: Dictionary of all enemies in the game that can be spawned at night

    /// <summary>
    /// The list of currently unlocked enemies that can be spawned.
    /// </summary>
    [SerializeField]
    private List<EnemySO> SpawnableEnemies;

    private List<EnemySO> unlockedEnemies;

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
    /// The distance the enemies will spawn from the base
    /// </summary>
    [SerializeField]
    private float spawnDistFromBase = 100f;

    /// <summary>
    /// The current amount of enemies that have been spawned
    /// </summary>
    private int spawnCount = 0;

    /// <summary>
    /// The base amount of enemies that can be spawned at once
    /// </summary>
    private int baseMaxSpawnCount = 20;


    /// <summary>
    /// This is called when the server starts running
    /// </summary>
    public override void OnStartServer()
    {
        //This subscribes the functions to the time manager so that when the events fire, these functions will trigger
        timeManager.OnNightStart += NightStarted;
        timeManager.OnNightEnd += NightEnded;

        unlockedEnemies = new List<EnemySO>();
        
    }

    /// <summary>
    /// This is called when the server stops running
    /// </summary>
    public override void OnStopServer()
    {
        //This unsubscribes the functions to the time manager so that they dont fire anymore
        timeManager.OnNightStart -= NightStarted;
        timeManager.OnNightEnd -= NightEnded;
    }

    /// <summary>
    /// Handles what happens at the start of the night cycle
    /// </summary>
    private void NightStarted()
    {
        UpdateUnlockedEnemies();

        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Handles what happens at the end of the night cycle/start of the day cycle
    /// </summary>
    private void NightEnded()
    {
        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;

            KillRemainingEnemies();
        }
    }

    /// <summary>
    /// A function that tells all remaining enemies that they should die off due to daytime
    /// </summary>
    private void KillRemainingEnemies()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>();

        foreach(Enemy enemy in enemies)
        {
            //NOTE: Probably dont want bosses to die here if we add them
            if(enemy.gameObject.activeSelf)
            {
                enemy.GameDeath();
            }
        }
    }

    /// <summary>
    /// The coroutine that handles the timing of enemy spawns
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies()
    {
        Debug.Log($"Enemy spawning started Health:{EnemyWaveScaling.EnemyHealthScaling(1f)}|Damage{EnemyWaveScaling.EnemyDamageScaling(1)}" +
            $"|MaxSpawns:{EnemyWaveScaling.MaxEnemySpawnScaling(baseMaxSpawnCount)}|MaxDensity:{EnemyWaveScaling.SpawnDensityScaling(10f)}");
        //This will just loop until the coroutine is stopped externally
        while (true)
        {
            //Checks to see if the spawned enemies is less than the current max spawned enemies
            if(spawnCount < EnemyWaveScaling.MaxEnemySpawnScaling(baseMaxSpawnCount))
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

        foreach (EnemySO enemy in cluster.enemies)
        {
            SpawnEnemy(enemy, cluster.radius, cluster.direction);
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

        //spawn enemy cluster in NESW directions
        newCluster.direction = Random.Range(0, 4);

        //TODO: Decide what the base density is going to be
        newCluster.minSpawnValue = EnemyWaveScaling.SpawnDensityScaling(10f);

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
            float enemyWeight = unlockedEnemies[randomIndex].EnemySpawnWeight;

            if(enemyWeight <= 0)
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
    private void SpawnEnemy(EnemySO enemySO, float spawnRadius, int direction)
    {
        //Used for updating values in the script once we spawn the enemy
        Enemy currentEnemy;

        //Get enemy spawn position
        Vector3 pos = GetSpawnPosition(spawnRadius, direction);

        //Spawn enemy (uses get enemy height halved due to pivot point being in middle, might need to change if assets are different)
        currentEnemy = Instantiate(enemySO.EnemyPrefab, pos + GetEnemyHeightHalved(enemySO.EnemyPrefab), Quaternion.identity).GetComponent<Enemy>();

        //Spawns the enemy on the client side
        ServerManager.Spawn(currentEnemy.gameObject);

        currentEnemy.InitializeValues();
        currentEnemy.currentHealth.Value = currentEnemy.EnemySO.EnemyHealth;

        currentEnemy.GetComponent<NetworkObject>().SetParent(this);

        //Increment spawn count
        spawnCount++;

        //Subscribe to enemy death event 
        currentEnemy.onEnemyKilled += ReturnEnemy;
    }

    private Vector3 GetSpawnPosition(float spawnRadius, int direction)
    {

        Vector3 cardinalPos;

        //Get the position based on the cardinal directions
        switch (direction)
        {
            case 0: //North
                cardinalPos = new Vector3(0, 0, spawnDistFromBase);
                break;
            case 1: //East
                cardinalPos = new Vector3(spawnDistFromBase, 0, 0);
                break;
            case 2: //South
                cardinalPos = new Vector3(0, 0, -spawnDistFromBase);
                break;
            case 3: //West
                cardinalPos = new Vector3(-spawnDistFromBase, 0, 0);
                break;
            default:
                return Vector3.zero;
        }

        //This offsets the spawn pos to a random point in the spawn radius
        //TODO: Will need to check to ensure this spawn point is not inside any other enemies or obstacles
        Vector2 offset = Random.insideUnitCircle * spawnRadius;

        //TODO: Need to use something like raycast to get ground height for spawn pos

        //Uses the cardinal position and offset to get the spawn position based on the heart crystal's position
        Vector3 heartCrystalPos = new Vector3(HeartCrystal.transform.position.x, 0f, HeartCrystal.transform.position.z);

        return heartCrystalPos + cardinalPos + new Vector3(offset.x, 0, offset.y);
    }

    /// <summary>
    /// This is used to ensure that the enemy spawns at the correct height, as they spawn from center of prefab
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private Vector3 GetEnemyHeightHalved(GameObject enemy)
    {
        return new Vector3(0, enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2f, 0);
    }

    /// <summary>
    /// Returns the enemy to the pool
    /// </summary>
    /// <param name="enemy"></param>
    public void ReturnEnemy(Enemy enemy)
    {
        spawnCount--;

        //Unsubscribe from event to avoid duplication
        enemy.onEnemyKilled -= ReturnEnemy;
    }

    /// <summary>
    /// Updates the currently unlocked enemies
    /// </summary>
    private void UpdateUnlockedEnemies()
    {
        //TODO: create a function that unlocks enemies based on conditions, probably have subscriptions to boss death events for example
        //This might need to be a whole class that handles progression

        //TODO: Probably make this more complicated in the future than 1 per day

        //This checks to make sure we dont have an out of bounds error
        if (SpawnableEnemies.Count >= timeManager.CurrentDay)
        {
            unlockedEnemies.Add(SpawnableEnemies[timeManager.CurrentDay]);
        }
    }
}
