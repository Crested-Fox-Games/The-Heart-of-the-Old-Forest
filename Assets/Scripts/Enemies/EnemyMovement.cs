using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    //The enemy movement will be done in multiple stages
    //1. The Brain will handle the decision making
    //2. The Pathfinder will figure out the best path to the heart crystal
    //3. The Movement will handle moving to the next location

    //References
    private NavMeshAgent agent;

    /// <summary>
    /// Sets the initial values for the script
    /// </summary>
    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Tells the nav mesh agent where to move to
    /// </summary>
    /// <param name="targetPos"></param>
    public void MovementTarget(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }
}
