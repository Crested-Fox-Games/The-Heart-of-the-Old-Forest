using UnityEngine;

/// <summary>
/// This interface will be put onto any buildings that the player is able to repair
/// </summary>
public interface IRepairable
{
    /// <summary>
    /// Checks if the structure has been destroyed
    /// </summary>
    /// <returns></returns>
    bool CanRepair();

    /// <summary>
    /// Rebuilds the structure on player interaction
    /// </summary>
    public void Rebuild();
}
