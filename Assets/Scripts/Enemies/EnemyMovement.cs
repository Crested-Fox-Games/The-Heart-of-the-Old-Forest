using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : NetworkBehaviour
{
    //The enemy movement will be done in multiple stages
    //1. The Brain will handle the decision making
    //2. The Pathfinder will figure out the best path to the heart crystal
    //3. The Movement will handle moving to the next location

    //References
    private NavMeshAgent agent;

    /// <summary>
    /// Runs on the client side
    /// </summary>
    public override void OnStartClient()
    {
        if(!IsServerStarted)
        {
            agent.enabled = false;
            return;
        }
    }

    private void Awake()
    {
        Initialize();
    }

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

        //Move this to a proper location later
        transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0f,-90f,0f);
    }
}
