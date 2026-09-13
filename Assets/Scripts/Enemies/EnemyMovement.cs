using FishNet.Object;
using System.Collections;
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

        //Changes the priority so that they dont push each other as much
        agent.avoidancePriority = Random.Range(30, 70);

        agent.updateRotation = true;

        agent.stoppingDistance = 0.8f;
    }

    /// <summary>
    /// Tells the nav mesh agent where to move to
    /// </summary>
    /// <param name="targetPos"></param>
    public void MovementTarget(GameObject targetObject)
    {
        if (!IsServerStarted )
        {
            return;
        }

        Collider targetCollider = targetObject.GetComponentInChildren<Collider>();

        if (targetCollider == null)
        {
            return;
        }

        //Update agent settings
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.isStopped = false;
        agent.updateRotation = true;

        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);

        closestPoint = new Vector3(closestPoint.x, 0.5f, closestPoint.z);

        //Set the enemy movement destination to the nearest part of obstruction collider
        if (NavMesh.SamplePosition(closestPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        //Move this to a proper location later
        //if (agent.velocity.sqrMagnitude > 0.0001f)
        //{
        //    transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0f, -0f, 0f);
        //} 
    }

    /// <summary>
    /// Tells the navmesh where to move based on an object
    /// </summary>
    /// <param name="targetObject"></param>
    public void MovementTargetActor(GameObject targetObject)
    {
        if(!IsServerStarted || targetObject == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.updateRotation = true;

        Vector3 targetPos = targetObject.transform.position;
        targetPos.y = transform.position.y;

        if(NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
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

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.isStopped = false;
        agent.updateRotation = true;

        targetPosition.y = transform.position.y;

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

        //if (agent.velocity.sqrMagnitude > 0.0001f)
        //{
        //    transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) * Quaternion.Euler(0f, -90f, 0f);
        //}
    }

    //Stop enemy movement completely
    public void StopMoving()
    {
        if (!IsServerStarted)
        {
            return;
        }

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;
    }

    public void PullTowards(Vector3 position, float distance, float time)
    {
        StartCoroutine(PullTowardsOverTime(position, distance, time));
    }

    private IEnumerator PullTowardsOverTime(Vector3 position, float distance, float time)
    {
        agent.isStopped = true;

        float timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            float pullDist = distance / time * Time.deltaTime;

            agent.Move(position * pullDist);

            yield return null;
        }

        float originalSpeed = agent.speed;

        agent.speed = 1f;

        //Reset the agent
        agent.isStopped = false;

        StartCoroutine(ChangeSpeedOverTime(agent.speed, originalSpeed, 2f));

        //Set the destination again
        agent.SetDestination(GetComponent<EnemyBrain>().CurrentTarget.TargetTransform.position);
    }

    private IEnumerator ChangeSpeedOverTime(float startingSpeed, float endingSpeed, float timeToAdjust)
    {
        float timer = 0f;

        while (timer < timeToAdjust)
        {
            timer += Time.deltaTime;

            float t = timer / timeToAdjust;

            agent.speed = Mathf.Lerp(startingSpeed, endingSpeed, t);

            yield return null;
        }

        agent.speed = endingSpeed;
    }

    public float GetRemainingDistance()
    {
        if (!IsServerStarted)
            return 0f;

        return agent.remainingDistance;
    }

}
