using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class KCollisionsFunction
{
    
    //Collisions
    private static float capsuleRadius;
    private static Vector3 capsuleP1;
    private static Vector3 capsuleP2;
    private static Vector3 capsuleCenterWorldPosition;
    private static RaycastHit hit; 
    private static bool collDetected;
    private static Vector3 CollisionVelocity;
    
    //Ground Checks 
    private static bool isGrounded;
    private static float gravityFactor;

    private static readonly int collisionMask =
        Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Gate"));
    
    //Collision Checks
    public static List<Vector3> CollisionAdjustMovementCords(Vector3 velocity, CapsuleCollider capsule, Rigidbody rb, bool useGravity, float gravity, bool enableSliding)
    {
        
        CollisionVelocity = velocity;
        var CAMC = new List<Vector3>(); //Creates a list of collision adjusted coordinates for the object to iterate through during movement
        CAMC.Add(rb.position); //Adds the current position of rigidbody 
        int count = 0; 
        
        //Compile Movement List
        while (CollisionVelocity.magnitude > 0.01f && count < 100) //ensures we're only adding coordinates as long as there's meaningful velocity left, or if too many bounces haven't occured 
        {
            count++;
            //Find Capsule Constants
            capsuleRadius = capsule.radius;
            capsuleCenterWorldPosition = capsule.center + CAMC.Last();
            capsuleP1 = new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
            capsuleP2 = -new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
            
            //Collision Check, boolean
            collDetected = Physics.CapsuleCast(capsuleP1, capsuleP2, capsuleRadius, CollisionVelocity, out hit,CollisionVelocity.magnitude, collisionMask, QueryTriggerInteraction.Ignore ); //Capsule Cast in direction and with magnitude of velocity 
            
            if(collDetected) //If the capsule cast collided with anything
            {
                if(hit.normal.y < -0.01) //if the collider hit a surface with a meaningful downward normal element (is a roof) then stop all movement, code still in progress so expect it to change
                {
                    CollisionVelocity = Vector3.zero;
                }
                else //Proceed if no roof collision 
                {
                    if(Mathf.Abs(hit.distance) > 0.1) //To ensure no clipping occurs due to machine arithmetic we make sure the collision distance has a high enough magnitude to remove rounding errors  
                    {
                        CAMC.Add(CAMC.Last() + CollisionVelocity.normalized*(Mathf.Abs(hit.distance) - 0.1f)); //Add point 0.1f away from collision point to list of coordinates to move to 
                        CollisionVelocity = -Vector3.Project(CollisionVelocity - CollisionVelocity.normalized*(Mathf.Abs(hit.distance) - 0.1f), hit.normal) + CollisionVelocity - (CollisionVelocity.normalized*(Mathf.Abs(hit.distance) - 0.1f)); //Figure out left over velocity and its direction using vector projections and surface normals
                    }
                    else //If the collision is too close to the object, still adjust the velocity but don't move any closer to the collision point
                    {
                        CollisionVelocity = -Vector3.Project(CollisionVelocity - CollisionVelocity.normalized*(Mathf.Abs(hit.distance) - 0.1f), hit.normal) + CollisionVelocity - (CollisionVelocity.normalized*(Mathf.Abs(hit.distance) - 0.1f)); //Handles small distance collisions by still adjusting the velocity without adding the required movement to the list
                    }
                }
                
            }
            else //Move player normally if no collision 
            {
                CAMC.Add(CAMC.Last() + CollisionVelocity);
                CollisionVelocity = Vector3.zero;
            }
        }

        return CAMC; //Return List 
    }
    
    //Grounded Check
    
    public static bool CheckGrounded(CapsuleCollider capsule, Vector3 rbPositon)
    {
        //Set Capsule Vars 
        capsuleCenterWorldPosition = capsule.center + rbPositon;
        capsuleP1 = new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
        capsuleP2 = -new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
        
        var grounded = Physics.CapsuleCast(capsuleP1, capsuleP2, capsuleRadius, new Vector3(0f,-1f,0f),0.10f, collisionMask, QueryTriggerInteraction.Ignore); //Cast Capsule straight down and return true if it hits something 
        return grounded;
    }
    
}
