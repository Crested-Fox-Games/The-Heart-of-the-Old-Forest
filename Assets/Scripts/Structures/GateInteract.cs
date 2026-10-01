using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;

public class GateInteract : NetworkBehaviour, IInteractable
{
    //private enum GateState
    //{
    //    CloseFront,
    //    CloseBack,
    //    OpenFront,
    //    OpenBack
    //}

    [SerializeField]
    private float interactTime = 0.25f;

    public float InteractTime => interactTime;

    /// <summary>
    /// Animator of the parent object
    /// </summary>
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// Door open or closed
    /// </summary>
    private bool isOpen = false;
    private bool gateAnim = false;

    /// <summary>
    /// Reference for the transform to determine the direction of the gate
    /// </summary>
    //[SerializeField]
    //private Transform gateDirection;

    /// <summary>
    /// Sync the gate state on the server side
    /// </summary>
    //private readonly SyncVar<GateState> gateState = new SyncVar<GateState>();

    //private void Awake()
    //{
    //    if (gateDirection == null)
    //    {
    //        gateDirection = transform;
    //    }
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();

        //gateState.OnChange += OnGateStateChanged;

        ////Set the state on the animator so it has the correct initial state
        //SetAnimatorState(gateState.Value);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        //gateState.OnChange -= OnGateStateChanged;
    }

    //private void OnGateStateChanged(GateState previous, GateState next, bool asServer)
    //{
    //    SetAnimatorState(next);
    //}
    
    ///// <summary>
    ///// Sets the state of the animator
    ///// </summary>
    ///// <param name="state"></param>
    //private void SetAnimatorState(GateState state)
    //{
    //    switch (state)
    //    {
    //        case GateState.CloseFront:
    //            animator.SetInteger("GateState", 0);
    //            break;
    //        case GateState.CloseBack:
    //            animator.SetInteger("GateState", 1);
    //            break;
    //        case GateState.OpenFront:
    //            animator.SetInteger("GateState", 2);
    //            break;
    //        case GateState.OpenBack:
    //            animator.SetInteger("GateState", 3);
    //            break;
    //    }
    //}

    public void Interact(NetworkObject player)
    {
        Debug.Log("Interaction started");
        if (!IsServerInitialized)
        {
            Debug.Log("Server is not initialiased");
            return;
        }

        if (player == null)
        {
            Debug.Log("player not found");
            return;
        }

        Debug.Log("Interaction occurring");
        ToggleGate(player);
    }

    /// <summary>
    /// Changes the state of the gate between open and closed
    /// </summary>
    /// <param name="player"></param>
    [Server]
    private void ToggleGate(NetworkObject player)
    {
        gateAnim = true;

        StartCoroutine(GateAnimation());

        //Vector3 directionToPlayer = player.transform.position - gateDirection.position;

        //float side = Vector3.Dot(gateDirection.forward, directionToPlayer);

        ////Close the gate if it is already open
        //if (gateState.Value != GateState.Closed)
        //{
        //    gateState.Value = GateState.Closed;
        //    return;
        //}

        //if (side >= 0)
        //{
        //    gateState.Value = GateState.OpenFront;
        //}
        //else
        //{
        //    gateState.Value = GateState.OpenBack;
        //}
    }

    private IEnumerator GateAnimation()
    {
        while (gateAnim == true)
        {
            Debug.Log("Entered toggle function");
            if (!isOpen)
            {
                animator.Play("GateOpenBack");
                yield return new WaitForSeconds(2f);
                isOpen = true;
                gateAnim = false;
            }
            else
            {
                animator.Play("GateCloseBack");
                yield return new WaitForSeconds(2f);
                isOpen = false;
                gateAnim = false;
            }
        }
    }

    public bool CanInteract(NetworkObject player)
    {
        return true;
    }
}
