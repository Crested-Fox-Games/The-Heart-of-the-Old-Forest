using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScriptv3 : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.8f;
    [SerializeField] private float rotationSmooth = 60f;
    [SerializeField] private float minPivotRotation = -30f;
    [SerializeField] private float maxPivotRotation = 65f;
    [SerializeField] private float defaultVelocity = 5f;
    [SerializeField] private float gravity = -0.001f;

    // --- CAMERA ---
    private float targetPlayerRotation;
    private float targetPivotRotation;
    private float currentPlayerRotation;
    private float currentPivotRotation;
    private Transform cameraPivot;

    // --- Components ---
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsule;
    
    // --- MOVEMENT ---
    private Vector3 movementInputVector;
    private bool isMoving;
    private bool isSliding;
    private bool isJumping;
    
    private Vector3 residualVelocity;
    private List<Vector3> movementCords;
    
    //Physics
    private bool isGrounded;
    private float gravityFactor;
    
    //Slide
    private Vector3 addedSlideVelocity;
    
    //Jump
    private Vector3 addedJumpVelocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        //Fetch Components 
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        cameraPivot = transform.Find("Camera Pivot");
        animator = GetComponentInChildren<Animator>();
        
        //Cursor Settings
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        //Turn off Physics
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
    }
    
    private void LateUpdate()
    {
        ApplyGravity(); //Applies Gravity
        MovePlayer(); //Moves The Player
        SetAnimationParameters(); //Sets Animation State
        UpdateCamera(); //Updates Camera 
    }

    // --- Inputs ---
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        movementInputVector = new Vector3(input.x, 0f, input.y);
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        if (delta.magnitude < 0.5f) delta = Vector2.zero;
        delta *= sensitivity * 0.0066f;

        targetPlayerRotation += delta.x;
        targetPivotRotation -= delta.y;
        targetPivotRotation = Mathf.Clamp(targetPivotRotation, minPivotRotation, maxPivotRotation);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(KCollisionsFunction.CheckGrounded(capsule, rb.position))
        {
            isJumping = true;
            residualVelocity.y = residualVelocity.y + 1f;
            Debug.unityLogger.Log("Jumped");
        }

    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        
        if (context.started && !isSliding)
        {
            isSliding = true;
            addedSlideVelocity = transform.TransformDirection(movementInputVector * 0.5f);
            residualVelocity = residualVelocity + addedSlideVelocity;
            
        }
        else if (context.canceled && isSliding)
        {
            isSliding = false;
            residualVelocity = residualVelocity - addedSlideVelocity;
        }
    }

    // --- CAMERA ---
    private void UpdateCamera()
    {
        float t = 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime);

        currentPlayerRotation = Mathf.Lerp(currentPlayerRotation, targetPlayerRotation, t);
        currentPivotRotation = Mathf.Lerp(currentPivotRotation, targetPivotRotation, t);

        rb.MoveRotation(Quaternion.Euler(0f, currentPlayerRotation, 0f));
        cameraPivot.localRotation = Quaternion.Euler(currentPivotRotation, 0f, 0f);
    }

    // --- MOVEMENT ---
    private Vector3 FindPlayerVelocity()
    {
        return InputVelocity() + residualVelocity;
    }
    

    private Vector3 InputVelocity()
    {
        if(movementInputVector.magnitude != 0)
        {
            return transform.TransformDirection(movementInputVector*defaultVelocity);
        }

        return Vector3.zero;
    }
    
    private void MovePlayer()
    {
        movementCords = KCollisionsFunction.CollisionAdjustMovementCords(FindPlayerVelocity(), capsule, rb, true, gravity ,true);
        
        for(int i = 0; i < movementCords.Count; i++) 
        { 
            rb.MovePosition(movementCords[i]);
        }
        
        movementCords.Clear();
        
    }

    //Animations
    private void SetAnimationParameters()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSliding", isSliding);
    }
    
    //Physics 

    private void ApplyGravity()
    {
        //Ground Check, boolean
        isGrounded = KCollisionsFunction.CheckGrounded(capsule, rb.position);
        
        //Enable gravity if not grounded
        if (!isGrounded)
        {
            residualVelocity.y = residualVelocity.y + gravity; //Add gravity factor to velocity
        }
        else
        {
            if (residualVelocity.y < 0f)
            {
                residualVelocity.y = 0f;   
            }
        }
    }
}

