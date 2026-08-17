using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    //Checks and stores all unique targets inside the trigger collider
    private HashSet<ITargetable> nearbyTargets = new HashSet<ITargetable>();

    /// <summary>
    /// Prevents other scripts from modifying this Hashset
    /// </summary>
    public IReadOnlyCollection<ITargetable> NearbyTargets => nearbyTargets;


    /// <summary>
    /// Called when a target enters the detection area
    /// </summary>
    public event Action<ITargetable> TargetEntered;

    /// <summary>
    /// Called when a target exits the detection area
    /// </summary>
    public event Action<ITargetable> TargetExited;


    /// <summary>
    /// Adds target to Hashset collection that enemy evaluates as possible new targets
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnterTargetable(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target == null)
        {
            return;
        }

        if (nearbyTargets.Add(target))
        {
            Debug.Log($"{gameObject.name} detected {other.gameObject.name}");

            TargetEntered?.Invoke(target);
        }
    }

    /// <summary>
    /// Removes target from Hashset collection that enemy evaluates as possible targets
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExitTargetable(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target == null)
        {
            return;
        }

        if (nearbyTargets.Remove(target))
        {
            Debug.Log($"{gameObject.name} lost {other.gameObject.name}");

            TargetExited?.Invoke(target);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollisionEnterTargetable(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnCollisionExitTargetable(other);
    }
}
