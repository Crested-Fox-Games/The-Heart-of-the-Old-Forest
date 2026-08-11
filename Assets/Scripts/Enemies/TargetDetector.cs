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

    private void OnTriggerEnter(Collider other)
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

    private void OnTriggerExit(Collider other)
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
}
