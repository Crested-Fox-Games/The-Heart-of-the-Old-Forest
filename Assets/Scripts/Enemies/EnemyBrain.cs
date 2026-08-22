using FishNet.Object;
using UnityEngine;

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

    private Vector3 blightSpawnPos;

    [SerializeField] private float blightRangeDistance;

    private enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Returning
    }

    private EnemyState currentState;


    // new enum state machine for blight enemy or wave enemy

    /// <summary>
    /// Initialise references before anything else
    /// </summary>
    private void Awake()
    {
        GetReferences();

        ReevaluateTargets();
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

    private void InitializeWaveEnemy()
    {
        SetTarget(defaultTarget);
    }

    private void InitializeBlightEnemy()
    {
        blightSpawnPos = transform.position;
        ChangeState(EnemyState.Idle);
    }

    /// <summary>
    /// Evaluate potential targets when a new target enters enemy collider
    /// </summary>
    /// <param name="target"></param>
    private void OnTargetEntered(ITargetable target)
    {
        Debug.Log($"EnemyBrain noticed a target entered: {target}");

        if (!enemy.IsWaveEnemy)
        {
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
        Debug.Log($"EnemyBrain noticed a target exited: {target}");

        if (!enemy.IsWaveEnemy)
        {
            if (currentTarget == target)
            {
                currentTarget = null;
                ChangeState(EnemyState.Idle);
            }
        }

        ReevaluateTargets();
    }

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

    private bool IsOutsideBlightLeash
        ()
    {
        float distance = Vector3.Distance(blightSpawnPos, transform.position);

        return distance >= blightRangeDistance;
    }

    private void UpdateBlightLeash()
    {
        if (currentState == EnemyState.Returning)
        {
            return;
        }

        if (IsOutsideBlightLeash())
        {
            ReturnToBlightSpawn();
        }
    }

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
        
        ITargetable obstruction = enemyPathfinder.FindDirectObstruction(defaultTarget);

        if (obstruction != null)
        {
            SetTarget(obstruction);
        }
        else
        {
            SetTarget(defaultTarget);
        }

        

        //foreach (ITargetable target in targetDetector.NearbyTargets)
        //{
        //    if (!target.IsAlive())
        //    {
        //        continue;
        //    }

        //    if (!target.IsAttackable())
        //    {
        //        continue;
        //    }
        //    Debug.Log($"Valid target: {target}");
        //}
    }

    private void Update()
    {
        if (!IsServerStarted)
        {
            return;
        }

        if (!enemy.IsWaveEnemy)
        {
            UpdateBlightLeash();
        }


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

    private void UpdateMoving()
    {
        if (!HasValidTarget())
        {
            ReevaluateTargets();
            return;
        }

        if (IsTargetInRange())
        {
            ChangeState(EnemyState.Attacking);
        }
    }

   private void UpdateAttacking()
    {
        if (!HasValidTarget())
        {
            ReevaluateTargets();
            return;
        }

        if (!IsTargetInRange())
        {
            ChangeState(EnemyState.Moving);
        }
    }

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
    private void EnterMoving()
    {
        Debug.Log($"Entering Moving state  {currentTarget.TargetTransform.gameObject}");

        enemyMovement.MovementTarget(currentTarget.TargetTransform.gameObject);
    }

    private void ExitMoving()
    {
        Debug.Log("Exiting Moving state");
    }

    private void EnterAttacking()
    {
        Debug.Log("Entering Attacking state");
        // connect to melee attack in enemymeleeclass
        enemyMovement.StopMoving();
        enemyMeleeClass.StartAttacking();
    }

    private void ExitAttacking()
    {
        Debug.Log("Exiting Attacking state");

        enemyMeleeClass.StopAttacking();
    }

    private void EnterReturning()
    {
        Debug.Log("Returning to blight spawn");

        enemyMeleeClass.StopAttacking();

        //TODO: update movement code
        enemyMovement.MovementTarget(blightSpawnPos);
    }

    private void UpdateReturning()
    {
        float distance = Vector3.Distance(transform.position, blightSpawnPos);

        if (distance <= 0.5f)
        {
            enemyMovement.StopMoving();

            ITargetable target = FindBlightTarget();

            ChangeState(EnemyState.Idle);
        }
    }

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

    private bool IsTargetInRange()
    {
        if (!HasValidTarget())
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.TargetTransform.position);

        return distance <= enemy.EnemyAttackRange;
    }
}
