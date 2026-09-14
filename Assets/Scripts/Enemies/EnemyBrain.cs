using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script handles all the enemies decision making
/// </summary>
public class EnemyBrain : NetworkBehaviour
{
    //TODO: This script will need to handle more complicated decision making later on.

    //TODO: Will need a way for the brain to run for night time enemies vs world enemies

    //References
    private Enemy enemy;
    private EnemyMovement enemyMovement;
    private TargetDetector targetDetector;
    private EnemyPathfinder enemyPathfinder;
    private EnemyMeleeClass enemyMeleeClass;

    //Heart crystal will be the default target position
    private ITargetable defaultTarget;
    //Any new target positions enemies choose to attack
    private ITargetable currentTarget;
    public ITargetable CurrentTarget => currentTarget;

    //Store blight enemy spawn
    private Vector3 blightSpawnPos;

    //Blight leash range
    [SerializeField] private float blightRangeDistance = 30f;

    //State machine
    private enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Returning
    }

    private EnemyState currentState;

    /// <summary>
    /// Initialise references before anything else
    /// </summary>
    private void Awake()
    {
        GetReferences();

        //ReevaluateTargets();
    }

    /// <summary>
    /// Gets the script references needed to run
    /// </summary>
    private void GetReferences()
    {
        enemy = GetComponent<Enemy>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyPathfinder = GetComponent<EnemyPathfinder>();
        targetDetector = GetComponentInChildren<TargetDetector>();
        enemyMeleeClass = GetComponent<EnemyMeleeClass>();
    }

    /// <summary>
    /// Subscribe to on trigger enter and exit events in targetDetector
    /// </summary>
    private void OnEnable()
    {
        if (targetDetector == null)
        {
            return;
        }

        targetDetector.TargetEntered += OnTargetEntered;
        targetDetector.TargetExited += OnTargetExited;
    }

    /// <summary>
    /// Unsubscribe to on trigger enter and exit events in targetDetector
    /// </summary>
    private void OnDisable()
    {
        if (targetDetector == null)
        {
            return;
        }

        targetDetector.TargetEntered -= OnTargetEntered;
        targetDetector.TargetExited -= OnTargetExited;
    }

    /// <summary>
    /// Initializes values through references and setting the target to the heart crystal
    /// </summary>
    /// <param name="heartCrystal"></param>
    public void Initialize(GameObject heartCrystal)
    {
        ITargetable target = heartCrystal.GetComponent<ITargetable>();

        if (target == null)
        {
            return;
        }

        defaultTarget = target;

        if (enemy.IsWaveEnemy)
        {
            InitializeWaveEnemy();
        }
        else
        {
            InitializeBlightEnemy();
        }
    }

    /// <summary>
    /// Initialise wave enemy state and movement target
    /// </summary>
    private void InitializeWaveEnemy()
    {
        SetTarget(defaultTarget);
    }

    /// <summary>
    /// Initialise blight enemy state
    /// </summary>
    private void InitializeBlightEnemy()
    {
        blightSpawnPos = transform.position;

        currentTarget = null;
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (!IsServerStarted)
        {
            return;
        }

        //Leash check for blight enemies
        if (!enemy.IsWaveEnemy)
        {
            UpdateBlightLeash();
        }

        //State machine yippeee!
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Moving:
                UpdateMoving();
                break;

            case EnemyState.Attacking:
                UpdateAttacking();
                break;
            case EnemyState.Returning:
                UpdateReturning();
                break;
        }
    }

    /// <summary>
    /// Evaluate potential targets when a new target enters enemy collider
    /// </summary>
    /// <param name="target"></param>
    private void OnTargetEntered(ITargetable target)
    {
        if (!enemy.IsWaveEnemy)
        {
            if (target == defaultTarget)
            {
                return;
            }

            SetBlightTarget(target);
            return;
        }

        ReevaluateTargets();
    }

    /// <summary>
    /// Evaluate potential targets when an existing target exits enemy collider
    /// </summary>
    /// <param name="target"></param>
    private void OnTargetExited(ITargetable target)
    {
        //Debug.Log($"EnemyBrain noticed a target exited: {target}");

        if (!enemy.IsWaveEnemy)
        {
            if (currentTarget == target)
            {
                currentTarget = null;

                if(FindBlightTarget() == null)
                {
                    ChangeState(EnemyState.Returning);
                }
                else
                {
                    SetBlightTarget(FindBlightTarget());
                }
            }
            return;
        }

        ReevaluateTargets();
    }

    /// <summary>
    /// Sets blight enemy target to any object implementing ITargetable (typically player)
    /// </summary>
    /// <param name="target"></param>
    private void SetBlightTarget(ITargetable target)
    {
        if (target == null)
        {
            return;
        }

        if (!target.IsAlive() || !target.IsAttackable())
        {
            return;
        }

        currentTarget = target;

        if (IsTargetInRange())
        {
            ChangeState(EnemyState.Attacking);
        }
        else
        {
            ChangeState(EnemyState.Moving);
        }
    }

    /// <summary>
    /// Returns blight enemy to their spawn position when they get far enough away from it
    /// </summary>
    /// <returns></returns>
    private bool IsOutsideBlightLeash()
    {
        float distance = Vector3.Distance(blightSpawnPos, transform.position);

        return distance >= blightRangeDistance;
    }

    /// <summary>
    /// Set the state to returning whenever the blight enemy is too far from its spawn
    /// </summary>
    private void UpdateBlightLeash()
    {
        if (currentState == EnemyState.Returning)
        {
            return;
        }

        if (IsOutsideBlightLeash())
        {
            Debug.Log("Outside blight leash detected");
            ReturnToBlightSpawn();
        }
    }

    /// <summary>
    /// Set state to returning 
    /// </summary>
    private void ReturnToBlightSpawn()
    {
        currentTarget = null;

        if (currentState != EnemyState.Returning)
        {
            ChangeState(EnemyState.Returning);
        }
    }

    /// <summary>
    /// Set a new target for the enemy to attack
    /// </summary>
    public void ReevaluateTargets()
    {
        if (defaultTarget == null)
        {
            return;
        }
        
        // Raycast to any obstructions between enemy and heart crystal
        ITargetable obstruction = enemyPathfinder.FindDirectObstruction(defaultTarget);

        if (obstruction != null)
        {
            SetTarget(obstruction);
        }
        else
        {
            SetTarget(defaultTarget);
        }
    }

    /// <summary>
    ///  Update enemy movement target based on their blight/wave status, when they enter the moving state
    /// </summary>
    private void UpdateMoving()
    {
        if (!HasValidTarget())
        {
            if (enemy.IsWaveEnemy)
            {
                ReevaluateTargets();
            }
            else
            {
                ChangeState(EnemyState.Returning);
            }

            return;
        }
        else
        {
            //Sets movement target based on if the current target is a player or not
            if (currentTarget.TargetTransform.GetComponent<PlayerRef>() != null)
            {
                enemyMovement.MovementTargetActor(currentTarget.TargetTransform.gameObject);
            }
            else
            {
                enemyMovement.MovementTarget(currentTarget.TargetTransform.gameObject);
            }
        }

        if (IsTargetInRange())
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        CheckIfStuck();
    }

    private float lastDist;
    private float stuckTimer;

    /// <summary>
    /// Checks to see if the enemy is stuck and cant get to its target
    /// </summary>
    private void CheckIfStuck()
    {
        float currentDist = enemyMovement.GetRemainingDistance();

        if(Mathf.Abs(currentDist - lastDist) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        lastDist = currentDist;

        if(stuckTimer >= 1f)
        {
            enemyMovement.StopMoving();
        }
    }


    /// <summary>
    /// Update attack logic whenever enemy is in attacking state
    /// </summary>
   private void UpdateAttacking()
    {
        if (!HasValidTarget())
        {
            if (enemy.IsWaveEnemy)
            {
                ReevaluateTargets();
            }
            else
            {
                ChangeState(EnemyState.Returning);
            }

            return;
        }

        RotateTowardsTarget();

        if (!IsTargetInRange())
        {
            ChangeState(EnemyState.Moving);
        }
    }

    /// <summary>
    /// Rotates enemy towards current target while attacking
    /// </summary>
    private void RotateTowardsTarget()
    {
        Vector3 direction = currentTarget.TargetTransform.position - transform.position;

        direction.y = 0;

        if (direction.magnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
    }

    /// <summary>
    /// Runs through all state change logic
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        ExitState(currentState);

        currentState = newState;

        EnterState(currentState);
    }

    /// <summary>
    /// Runs necessary functions when enemy enters a specific state
    /// </summary>
    /// <param name="state"></param>
    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Moving:
                EnterMoving();
                break;

            case EnemyState.Attacking:
                EnterAttacking();
                break;
            case EnemyState.Returning:
                EnterReturning();
                break;
        }
    }

    /// <summary>
    /// Runs necessary functions when enemy ecits a specific state
    /// </summary>
    /// <param name="state"></param>
    private void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Moving:
                ExitMoving();
                break;

            case EnemyState.Attacking:
                ExitAttacking();
                break;
        }
    }

    /// <summary>
    /// Runs necessary logic when enemy enters moving state
    /// </summary>
    private void EnterMoving()
    {
        if (!IsOutsideBlightLeash())
        {
            Debug.Log($"Entering Moving state  {currentTarget.TargetTransform.gameObject}");

            //Moves based on whether target is a player or not
            if (currentTarget.TargetTransform.GetComponent<PlayerRef>() != null)
            {
                enemyMovement.MovementTargetActor(currentTarget.TargetTransform.gameObject);
            }
            else
            {
                enemyMovement.MovementTarget(currentTarget.TargetTransform.gameObject);
            }

            //RotateTowardsTarget();
        }
    }

    /// <summary>
    /// Runs necessary logic when enemy exits moving state
    /// </summary>
    private void ExitMoving()
    {
        Debug.Log("Exiting Moving state");
        // connect to melee attack in enemymeleeclass
        enemyMovement.StopMoving();
    }

    /// <summary>
    /// Runs necessary logic when enemy enters attacking state
    /// </summary>
    private void EnterAttacking()
    {
        Debug.Log("Entering Attacking state");
        
        enemyMeleeClass.StartAttacking();
    }

    /// <summary>
    /// Runs necessary logic when enemy exits attacking state
    /// </summary>
    private void ExitAttacking()
    {
        Debug.Log("Exiting Attacking state");

        enemyMeleeClass.StopAttacking();
    }

    /// <summary>
    /// Runs necessary logic when the enemy enters returning to blight state
    /// </summary>
    private void EnterReturning()
    {
        Debug.Log("Returning to blight spawn");

        enemyMovement.MovementTarget(blightSpawnPos);
    }

    /// <summary>
    /// Runs necessary logic when the enemy is in the returning state
    /// </summary>
    private void UpdateReturning()
    {
        float distance = Vector3.Distance(transform.position, blightSpawnPos);

        if (distance <= 0.5f)
        {
            return;
        }

        ITargetable target = FindBlightTarget();

        if (target != null)
        {
            SetBlightTarget(target);
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
    }

    /// <summary>
    /// Finds a valid target within the target detector hitbox
    /// </summary>
    /// <returns></returns>
    private ITargetable FindBlightTarget()
    {
        foreach (ITargetable target in targetDetector.NearbyTargets)
        {
            if (!target.IsAlive())
            {
                continue;
            }

            if (!target.IsAttackable())
            {
                continue;
            }

            return target;
        }

        return null;
    }

    /// <summary>
    /// Sets the current target of the enemy
    /// </summary>
    /// <param name="target"></param>
    private void SetTarget(ITargetable target)
    {
        currentTarget = target;

        if (!IsServerStarted)
            return;

        if (IsTargetInRange())
        {
            ChangeState(EnemyState.Attacking);
        }
        else
        {
            ChangeState(EnemyState.Moving);
        }
    }

    /// <summary>
    /// Checks if the enemy's target is valid by running through ITargetable logic
    /// </summary>
    /// <returns></returns>
    private bool HasValidTarget()
    {
        if (currentTarget == null)
        {
            return false;
        }

        if (!currentTarget.IsAlive())
        {
            return false;
        }

        if (!currentTarget.IsAttackable())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the enemy's target is within attacking range
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTargetInRange(ITargetable target)
    {
        if (target == null || !target.IsAlive())
        {
            return false;
        }

        Collider targetCollider = target.TargetTransform.GetComponentInChildren<Collider>();

        if (targetCollider == null)
        {
            return false;
        }

        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);

        float distance = Vector3.Distance(transform.position, closestPoint);

        return distance <= enemy.EnemyAttackRange;
    }

    /// <summary>
    /// Returns if the target is in range without a targetable input above
    /// </summary>
    /// <returns></returns>
    private bool IsTargetInRange()
    {
        return IsTargetInRange(currentTarget);
    }
}
