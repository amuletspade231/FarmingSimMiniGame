using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    private NewControls controls;
    public float moveSpeed = 5f; // Speed for object movement

    // Stores direction to move
    private Vector3 moveDirection = Vector3.zero;

    // Indicates if were handling 2D or 3D movement
    public bool is2D = true;

    private void Awake()
    {
        controls = new NewControls(); // Set uup the input controls
    }

    private void OnEnable()
    {
        controls.Enable(); // Enable controls

        // Movement starts and stops here
        controls.Player.Movement.performed += Movement;
        controls.Player.Movement.canceled += StopMovement;

        // Interaction input
        controls.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        // Unbind everything when disablingg
        controls.Player.Movement.performed -= Movement;
        controls.Player.Movement.canceled -= StopMovement;
        controls.Player.Interact.performed -= OnInteract;

        controls.Disable(); // Disable controls
    }

    // Handles movement input left or right only
    private void Movement(InputAction.CallbackContext context)
    {
        float moveInput = 0f; // Default no movement

        // Check which button was pressed - left or right
        if (context.control == Keyboard.current.leftArrowKey || context.control == Gamepad.current?.dpad.left)
        {
            moveInput = -1f;  // Move left
        }
        else if (context.control == Keyboard.current.rightArrowKey || context.control == Gamepad.current?.dpad.right)
        {
            moveInput = 1f;   // Move right
        }

        // Set movement for 2D (X-axis) or 3D (X-axis, Z could be added later if needed)
        moveDirection = is2D ? new Vector3(moveInput, 0, 0) : new Vector3(moveInput, 0, 0);
    }

    // Stops movement when key is released
    private void StopMovement(InputAction.CallbackContext context)
    {
        moveDirection = Vector3.zero;  // Stop movement
    }

    // Triggers on interaction input 
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.control == Gamepad.current?.buttonSouth)
        {
            Debug.Log("Gamepad South Button pressed"); // Interaction feedback
        }
        else if (context.control == Keyboard.current?.spaceKey)
        {
            Debug.Log("Keyboard Spacebar pressed"); // Interaction feedback
        }
    }

    // Moves the object based on the input direction every frame
    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime); // Apply movement
    }
}
