using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScriptv3 : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.8f;
    [SerializeField] private float rotationSmooth = 60f;
    [SerializeField] private float minPivotRotation = -30f;
    [SerializeField] private float maxPivotRotation = 65f;
    [SerializeField] private float defaultVelocity = 0.5f;

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
    private Vector3 playerVelocity;
    private  Vector3 residualVelocity;
    
    //Slide
    private Vector3 addedSlideVelocity;
    
    // --- PHYSICS ---
    private float gravity;

    // --- COLLISION ---
    private float capsuleRadius;
    private Vector3 capsuleP1;
    private Vector3 capsuleP2;
    private Vector3 capsuleCenterWorldPosition;
    
    
    
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
        
        //Set Vars
        SetCapsuleVars();
        
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
        FindPlayerVelocity();
        SetAnimationParameters();
        UpdateCamera();
        MovePlayer();
    }

    // --- INPUT ---
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

    public void OnSlide(InputAction.CallbackContext context)
    {
        print("OnSlide");
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
        playerVelocity = Vector3.Lerp(playerVelocity, InputVelocity() + residualVelocity, 1f - Mathf.Exp(-10* Time.deltaTime));
        return playerVelocity;
        
    }

    private Vector3 InputVelocity()
    {
        if (movementInputVector.magnitude != 0)
        {
            return transform.TransformDirection(movementInputVector*defaultVelocity);
        }

        return Vector3.zero;
    }
    

    private void MovePlayer()
    {
        
        rb.MovePosition(playerVelocity + rb.position);
        if (playerVelocity.magnitude > 0.1 && !isMoving)
        {
            isMoving = true;
        }
        else if (playerVelocity.magnitude < 0.1 && isMoving)
        {
            isMoving = false;
        }


    }

    private void SetAnimationParameters()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSliding", isSliding);
    }
    
    
    // --- COLLISIONS ---
    
    private void SetCapsuleVars()
    {
        capsule = GetComponent<CapsuleCollider>();
        capsuleRadius = capsule.radius;
        capsuleCenterWorldPosition = capsule.center + rb.position;
        capsuleP1 = new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
        capsuleP2 = -new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
    }

    private void CheckCollisions()
    {
        Physics.CapsuleCast(capsuleP1, capsuleP2, capsuleRadius, playerVelocity, playerVelocity.magnitude);
    }
}

