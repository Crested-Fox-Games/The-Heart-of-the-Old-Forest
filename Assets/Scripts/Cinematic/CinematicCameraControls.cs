using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicCameraControls : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float lookSpeed = 0.1f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    //Inputs
    [SerializeField]
    private InputActionAsset inputActions;

    private InputActionMap cinematicMovementMap;

    private InputAction movement;
    private InputAction vertical;
    private InputAction look;

    //Subscriptions
    private void Start()
    {
        cinematicMovementMap = inputActions.FindActionMap("CinematicMovement");

        cinematicMovementMap.Disable();

        movement = cinematicMovementMap.FindAction("Movement");
        vertical = cinematicMovementMap.FindAction("Vertical");
        look = cinematicMovementMap.FindAction("Look");
    }


    private void Update()
    {
        Vector2 moveInput = movement.ReadValue<Vector2>();
        float verticalInput = -vertical.ReadValue<float>();

        Vector3 moveDirection = new Vector3(moveInput.x, verticalInput, moveInput.y);

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        Vector2 lookInput = look.ReadValue<Vector2>();

        float mouseX = lookInput.x * lookSpeed;
        float mouseY = lookInput.y * lookSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;
        yRotation = Mathf.Repeat(yRotation, 360f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
