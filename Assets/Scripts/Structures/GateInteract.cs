using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class NewNetworkBehaviourTemplate : NetworkBehaviour, IInteractable
{
    private enum GateState
    {
        Closed,
        OpenFront,
        OpenBack
    }

    /// <summary>
    /// Animator of the parent object
    /// </summary>
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// Reference for the transform to determine the direction of the gate
    /// </summary>
    [SerializeField]
    private Transform gateDirection;

    /// <summary>
    /// Sync the gate state on the server side
    /// </summary>
    private readonly SyncVar<GateState> gateState = new SyncVar<GateState>();

    public float InteractTime => throw new System.NotImplementedException();

    private void Awake()
    {
        if (gateDirection == null)
        {
            gateDirection = transform;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        gateState.OnChange += OnGateStateChanged;

        //Set the state on the animator so it has the correct initial state
        SetAnimatorState(gateState.Value);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        gateState.OnChange -= OnGateStateChanged;
    }

    private void OnGateStateChanged(GateState previous, GateState next, bool asServer)
    {
        SetAnimatorState(next);
    }
    
    private void SetAnimatorState(GateState state)
    {
        switch (state)
        {
            case GateState.Closed:
                animator.SetInteger("GateState", 0);
                break;
            case GateState.OpenFront:
                animator.SetInteger("GateState", 1);
                break;
            case GateState.OpenBack:
                animator.SetInteger("GateState", 2);
                break;
        }
    }

    public void Interact(NetworkObject player)
    {
        if (!IsServerInitialized)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        ToggleGate(player);
    }

    [Server]
    private void ToggleGate(NetworkObject player)
    {
        //Close the gate if it is already open
        if (gateState.Value != GateState.Closed)
        {
            gateState.Value = GateState.Closed;
            return;
        }

        Vector3 directionToPlayer = player.transform.position - gateDirection.position;

        float side = Vector3.Dot(gateDirection.forward, directionToPlayer);

        if (side >= 0)
        {
            gateState.Value = GateState.OpenFront;
        }
        else
        {
            gateState.Value = GateState.OpenBack;
        }
    }

    public bool CanInteract(NetworkObject player)
    {
        throw new System.NotImplementedException();
    }
}
