using Unity.VisualScripting;
using UnityEngine;

public static class EnemyWaveScaling
{

    /// <summary>
    /// The factor that the day is multiplied by
    /// </summary>
    private static float healthFactor = 0.25f;

    /// <summary>
    /// The factor that the day is multiplied by
    /// </summary>
    private static float damageFactor = 0.25f;

    /// <summary>
    /// The factor that scales the amount of enemies spawned based on nights
    /// </summary>
    private static float spawnFactorNight = 1f;

    /// <summary>
    /// The factor that scales the amount wave clusters can spawn based on nights
    /// </summary>
    private static float spawnDensityFactorNight = 1f;

    /// <summary>
    /// When we do testing, we will adjust this number to ensure that there isnt too much lag on either the pc or network
    /// </summary>
    private static float AbsoluteMaxEnemySpawns = 1000f;

    /// <summary>
    /// Calculate the enemies health based on different factors
    /// </summary>
    /// <param name="baseHealth"></param>
    /// <returns></returns>
    public static float EnemyHealthScaling(float baseHealth)
    {
        //Scale by x% each night
        float scaledHealth = baseHealth * (1 + TimeManager.Instance.CurrentDay * healthFactor);

        return scaledHealth;
    }

    /// <summary>
    /// Calculate the enemies damage based on different factors
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public static float EnemyDamageScaling(float baseDamage)
    {
        //Scale by x% each night
        float scaledDamage = baseDamage * (1 + TimeManager.Instance.CurrentDay * damageFactor);

        return scaledDamage;
    }

    /// <summary>
    /// Calculate the max enemies spawned by waves at once
    /// </summary>
    /// <param name="baseSpawns"></param>
    /// <returns></returns>
    public static float MaxEnemySpawnScaling(float baseSpawns)
    {
        float scaledSpawns = baseSpawns * (1 + TimeManager.Instance.CurrentDay * spawnFactorNight);

        return Mathf.Min(scaledSpawns, AbsoluteMaxEnemySpawns);
    }

    /// <summary>
    /// Calculate the density wave clusters spawn with
    /// </summary>
    /// <param name="baseDensity"></param>
    /// <returns></returns>
    public static float SpawnDensityScaling(float baseDensity)
    {
        float scaledDensity = baseDensity * (1 + TimeManager.Instance.CurrentDay * spawnDensityFactorNight);

        return scaledDensity;
    }
}
