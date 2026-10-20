using UnityEngine;

/// <summary>
/// Checks to see if an object meets all the requirements to be targeted by an enemy
/// </summary>
public interface ITargetable
{
    Transform TargetTransform { get; }

    /// <summary>
    /// Checks if target is still active in the scene
    /// </summary>
    /// <returns></returns>
    bool IsAlive();

    /// <summary>
    /// Checks if target can be attacked by enemy
    /// </summary>
    /// <returns></returns>
    bool IsAttackable();

    /// <summary>
    /// Handles taking damage from enemies
    /// </summary>
    /// <param name="damage"></param>
    bool TakeDamage(float damage);
}
