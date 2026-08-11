using UnityEngine;

public static class EnemyWaveScaling
{
    //TODO: Implement a way for the enemy class to know if it is a wave enemy or a blight enemy
    //TODO: types of enemies that spawn based on nights
    //TODO: max enemy spawn amount based on nights and blight nodes cleared
    //TODO: spawn density based on nights and blight nodes cleared


    /// <summary>
    /// Calculate the enemies health based on different factors
    /// </summary>
    /// <param name="baseHealth"></param>
    /// <returns></returns>
    public static float EnemyHealthScaling(float baseHealth)
    {
        //Scale by x% each night
        return baseHealth;
    }

    public static float EnemyDamageScaling(float baseDamage)
    {
        //Scale by x% each night
        return baseDamage;
    }
}
