using UnityEngine;

/// <summary>
/// This script handles all the enemies decision making
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    //TODO: This script will need to handle more complicated decision making later on.

    //TODO: Will need a navigation script for handling the pathfinding.

    //References
    private Enemy enemy;
    private EnemyMovement enemyMovement;

    //For now this will just be the heart crystal
    private GameObject currentTarget;

    /// <summary>
    /// Gets the script references needed to run
    /// </summary>
    private void GetReferences()
    {
        enemy = GetComponent<Enemy>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    /// <summary>
    /// Allows the enemy class to pass through the heart crystal
    /// </summary>
    /// <param name="heartCrystal"></param>
    public void Initialize(GameObject heartCrystal)
    {
        GetReferences();
        SetTarget(heartCrystal);
    }

    /// <summary>
    /// Sets the current target of the enemy
    /// </summary>
    /// <param name="target"></param>
    private void SetTarget(GameObject target)
    {
        currentTarget = target;

        //TODO: Will probably need to move this later
        enemyMovement.MovementTarget(currentTarget.transform.position);
    }
}
