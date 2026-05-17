using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //TODO: Some sort of dictionary for spawning enemies based on conditions like bosses killed, amount of days passed, total player count

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

        yield return null;
    }

    private void SpawnEnemy()
    {

    }
}
