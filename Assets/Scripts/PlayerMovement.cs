using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.8f;
    [SerializeField] private float rotationSmooth = 60f;
    [SerializeField] private float minPivotRotation = -30f;
    [SerializeField] private float maxPivotRotation = 65f;
    [SerializeField] private float defaultVelocity = 0.5f;

    // --- CAMERA ---
    /// <summary>
    /// The rotation set by input, used to lerp for smooth movement
    /// </summary>
    private float targetPlayerRotation;
    /// <summary>
    /// The rotation of the pivot point set by input, used to lerp for smooth movement
    /// </summary>
    private float targetPivotRotation;
    /// <summary>
    /// The current rotation of the player, used to lerp for smooth movement
    /// </summary>
    private float currentPlayerRotation;
    /// <summary>
    /// The current rotation of the pivot point, used to lerp for smooth movement
    /// </summary>
    private float currentPivotRotation;
    private Transform cameraPivot;

    // --- COMPONENTS ---
    
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsule;

    // --- PLAYER INPUTS ---
    /// <summary>
    /// The action map for all of the player inputs
    /// </summary>
    private InputActionMap playerMap;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction slideAction;
    
    // --- MOVEMENT ---
    /// <summary>
    /// Holds the Vector3 of the players movement inputs
    /// </summary>
    private Vector3 movementInputVector;
    private bool isMoving;
    private bool isSliding;
    private Vector3 playerVelocity;
    private Vector3 residualVelocity;
    
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

        //Gets the players action map
        playerMap = InputSystem.actions.FindActionMap("Player");

        //Finds the different player inputs
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        slideAction = playerMap.FindAction("Slide");

        //Subsribes functions to the player inputs
        SubscribeToInputs();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInputs();
    }

    private void SubscribeToInputs()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        lookAction.performed += OnCameraMovement;
        lookAction.canceled += OnCameraMovement;

        slideAction.started += OnSlide;
        slideAction.canceled += OnSlide;
    }

    private void UnsubscribeFromInputs()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        lookAction.performed -= OnCameraMovement;
        lookAction.canceled -= OnCameraMovement;
        
        slideAction.started -= OnSlide;
        slideAction.canceled -= OnSlide;
    }
    
    /// <summary>
    /// Sets the initial values for the components
    /// </summary>
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
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        
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
    /// <summary>
    /// Handles the players movement inputs, doesn't do any movement itself
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //Gets the values from the player input
        Vector2 input = context.ReadValue<Vector2>();

        //Sets the inputs to Vector3 for the movement function
        movementInputVector = new Vector3(input.x, 0f, input.y);
    }

    /// <summary>
    /// Handles the rotation of the player when the camera moves
    /// </summary>
    /// <param name="context"></param>
    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        //Gets the delta between the last 2 updates
        Vector2 delta = context.ReadValue<Vector2>();

        //Ensures that if the magnitude is small it will not be jittery
        if (delta.magnitude < 0.5f)
        {
            delta = Vector2.zero;
        }

        //Delta multiplied by sensitivity and (a random number?)
        delta *= sensitivity * 0.0066f;

        //Updates the player rotation and pivot rotation based on delta values
        targetPlayerRotation += delta.x;
        targetPivotRotation -= delta.y;

        //Clamps the pivot rotation to stop the camera clipping
        targetPivotRotation = Mathf.Clamp(targetPivotRotation, minPivotRotation, maxPivotRotation);
    }

    /// <summary>
    /// Handles the player sliding
    /// </summary>
    /// <param name="context"></param>
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
    /// <summary>
    /// Handles the camera movement
    /// </summary>
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

    /// <summary>
    /// Gets the players velocity
    /// </summary>
    /// <returns></returns>
    private Vector3 InputVelocity()
    {
        if (movementInputVector.magnitude != 0)
        {
            return transform.TransformDirection(movementInputVector * defaultVelocity);
        }

        return Vector3.zero;
    }
    
    /// <summary>
    /// Handles the actual movement of the player
    /// </summary>
    private void MovePlayer()
    {
        //Uses the rigidbody to move the players position based on the player velocity
        rb.MovePosition(playerVelocity + rb.position);

        //Updates the boolean for the animator
        if (playerVelocity.magnitude > 0.1 && !isMoving)
        {
            isMoving = true;
        }
        else if (playerVelocity.magnitude < 0.1 && isMoving)
        {
            isMoving = false;
        }
    }

    /// <summary>
    /// Handles updating the animator of the player
    /// </summary>
    private void SetAnimationParameters()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSliding", isSliding);
    }
    
    
    // --- COLLISIONS ---
    
    /// <summary>
    /// Creates the colliders for collisions
    /// </summary>
    private void SetCapsuleVars()
    {
        capsule = GetComponent<CapsuleCollider>();
        capsuleRadius = capsule.radius;
        capsuleCenterWorldPosition = capsule.center + rb.position;
        capsuleP1 = new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
        capsuleP2 = -new Vector3(0f, (capsule.height / 2 - capsuleRadius), 0f) + capsuleCenterWorldPosition;
    }

    /// <summary>
    /// Checks to see if any collisions have occured
    /// </summary>
    private void CheckCollisions()
    {
        //TODO: Make this work
        Physics.CapsuleCast(capsuleP1, capsuleP2, capsuleRadius, playerVelocity, playerVelocity.magnitude);
    }
}

