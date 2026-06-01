using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySpawner : MonoBehaviour
{
    //TODO: Some sort of dictionary for spawning enemies based on conditions like bosses killed, amount of days passed, total player count
    
    //NOTE: THIS IS A TESTING PREFAB, Needs to be updated later
    [SerializeField]
    private GameObject enemyPrefab;

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
    }

    /// <summary>
    /// The coroutine that handles the timing of enemy spawns
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies()
    {
        //TODO: change this to be based on the amount of enemies spawned, so that we can cap the amount. 
        //This means that we will need to either have the enemies be children of this object and check child count,
        //or we need to have some sort of subscription system that fires on enemy death 

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(baseSpawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Enemy currentEnemy;
        //TODO: calculate the spawn position based on how the base is set up
        //(dont want static distance from crystal as it may spawn enemies in base)

        //TODO: select enemy type

        //Spawn enemy
        currentEnemy = Instantiate(enemyPrefab, transform.position + GetEnemyHeightHalved(enemyPrefab), Quaternion.identity).GetComponent<Enemy>();

        //Set heart crystal as target for enemy
        currentEnemy.SetHeartCrystal(HeartCrystal);

        //TODO: subscribe to enemy death event? (If limiting amount of enemies spawned at once)
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
}
