using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    //Radius of enemy raycast
    [SerializeField] private float castRadius = 0.5f;

    //Layer the enemy raycasts to
    [SerializeField] private LayerMask structureLayer;

    //private Vector3 gizmoOrigin;
    //private Vector3 gizmoDirection;
    //private float gizmoDistance;
    //private bool gizmoHit;
    //private Vector3 gizmoHitPoint;

    public ITargetable FindDirectObstruction(ITargetable destination)
    {
        if (destination == null)
        {
            return null;
        }

        Vector3 origin = transform.position;
        Vector3 targetPosition = destination.TargetTransform.position;

        Vector3 direction = targetPosition - origin;
        float distance = direction.magnitude;

        if (distance <= 0.01f)
        {
            return null;
        }

        direction.Normalize();

        //// Store information for the gizmo
        //gizmoOrigin = origin;
        //gizmoDirection = direction;
        //gizmoDistance = distance;
        //gizmoHit = false;

        if (Physics.SphereCast(
            origin,
            castRadius,
            direction,
            out RaycastHit hit,
            distance,
            structureLayer))
        {
            //gizmoHit = true;
            //gizmoHitPoint = hit.point;

            ITargetable target = hit.collider.GetComponentInParent<ITargetable>();

            if (target == null)
            {
                return null;
            }

            if (!target.IsAlive())
            {
                return null;
            }

            if (!target.IsAttackable())
            {
                return null;
            }

            return target;
        }

        return null;
    }

    //private void OnDrawGizmos()
    //{
    //    if (gizmoDistance <= 0f)
    //    {
    //        return;
    //    }

    //    Gizmos.color = gizmoHit ? Color.red : Color.green;

    //    Vector3 end = gizmoOrigin + gizmoDirection * gizmoDistance;

    //    Gizmos.DrawWireSphere(gizmoOrigin, castRadius);
    //    Gizmos.DrawWireSphere(end, castRadius);
    //    Gizmos.DrawLine(gizmoOrigin, end);

    //    if (gizmoHit)
    //    {
    //        Gizmos.DrawSphere(gizmoHitPoint, 0.1f);
    //    }
    //}
}
