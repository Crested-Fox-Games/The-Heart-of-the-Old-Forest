using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        agent.enabled = IsServerStarted;

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
        //agent.stoppingDistance = 4f;
    }

    /// <summary>
    /// Tells the nav mesh agent where to move to
    /// </summary>
    /// <param name="targetPos"></param>
    public void MovementTarget(GameObject targetObject)
    {
        if (!IsServerStarted)
        {
            return;
        }

        Collider targetCollider = targetObject.GetComponentInChildren<Collider>();

        if (targetCollider == null)
        {
            return;
        }

        agent.isStopped = false;

        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);

        closestPoint = new Vector3(closestPoint.x, 0.5f, closestPoint.z);

        //Set the enemy movement destination to the nearest part of obstruction collider
        if (NavMesh.SamplePosition(closestPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        //Move this to a proper location later
        if (agent.velocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0f, -90f, 0f);
        } 
    }

    /// <summary>
    /// Update target the enemy needs to move towards
    /// </summary>
    /// <param name="targetPosition"></param>
    public void MovementTarget(Vector3 targetPosition)
    {
        if (!IsServerStarted)
        {
            return;
        }

        agent.isStopped = false;

        targetPosition = new Vector3(targetPosition.x, 0.5f, targetPosition.z);

        //Set the updated destination of the enemy
        SetMovementDestination(targetPosition);
    }

    /// <summary>
    /// Sets the movement position the enemy needs to move towards
    /// </summary>
    /// <param name="targetPosition"></param>
    private void SetMovementDestination(Vector3 targetPosition)
    {
        //Update the navmesh for enemy's movement target
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        if (agent.velocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0f, -90f, 0f);
        }
    }

    //Stop enemy movement completely
    public void StopMoving()
    {
        if (!IsServerStarted)
        {
            return;
        }

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
