using UnityEngine;

public static class EnemyWaveScaling
{
    //TODO: Implement a way for the enemy class to know if it is a wave enemy or a blight enemy
    //TODO: types of enemies that spawn based on nights
    //TODO: max enemy spawn amount based on nights and blight nodes cleared
    //TODO: spawn density based on nights and blight nodes cleared

    /// <summary>
    /// The factor that the day is multiplied by
    /// </summary>
    private static float healthFactor = 0.25f;

    /// <summary>
    /// The factor that the day is multiplied by
    /// </summary>
    private static float damageFactor = 0.25f;

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
}
