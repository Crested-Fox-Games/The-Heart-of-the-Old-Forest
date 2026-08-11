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

    private void OnTriggerEnter(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            return;
        }

        nearbyTargets.Add(target);

        Debug.Log($"{gameObject.name} detected {other.gameObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        ITargetable target = other.GetComponent<ITargetable>();

        if (target != null)
        {
            return;
        }

        nearbyTargets.Remove(target);

        Debug.Log($"{gameObject.name} lost {other.gameObject.name}");
    }
}
