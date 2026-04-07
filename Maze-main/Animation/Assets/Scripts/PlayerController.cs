using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 700f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public LayerMask groundMask = 1; // What counts as ground
    public Transform groundCheck; // Empty GameObject positioned at character's feet
    public float groundCheckRadius = 0.2f;

    [Header("Camera Settings")]
    public float cameraRotationSpeed = 100f;
    public bool enableCameraRotation = true;

    [Header("Animation")]
    public Animator animator;

    [Header("Camera Reference")]
    public Transform cameraTransform; // Reference to the camera

    [Header("Input Keys")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;

    // Private variables
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isMoving = false;
    private bool isRunning = false;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float horizontalInput;
    private float verticalInput;
    private float currentMoveSpeed;

    // Animation parameter names
    private string walkAnimParam = "isWalking";
    private string idleAnimParam = "isIdle";
    private string runAnimParam = "isRunning";
    private string jumpAnimParam = "isJumping";
    private string groundedAnimParam = "isGrounded";
    private string speedAnimParam = "Speed";

    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();

        // If animator is not assigned, try to get it from this gameobject
        if (animator == null)
            animator = GetComponent<Animator>();

        // If camera transform is not assigned, find the main camera
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                cameraTransform = mainCamera.transform;
        }

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0); // Adjust Y based on your character
            groundCheck = groundCheckObj.transform;
        }

        // Freeze rotation on X and Z axes to prevent character from falling over
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        currentMoveSpeed = walkSpeed;
    }

    void Update()
    {
        // Check if grounded
        CheckGrounded();
        
        // Get input
        HandleInput();
        
        // Handle camera rotation
        HandleCameraRotation();
        
        // Handle animations
        HandleAnimations();
    }

    void FixedUpdate()
    {
        // Handle movement in FixedUpdate for physics
        HandleMovement();
    }

    void CheckGrounded()
    {
        // Check if character is on ground using a sphere cast
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        
        // Reset jumping flag when landing
        if (isGrounded && isJumping)
        {
            isJumping = false;
        }
    }

    void HandleInput()
    {
        // Get movement input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Check for running input
        isRunning = Input.GetKey(runKey) && isMoving && isGrounded;

        // Set current move speed based on running state
        currentMoveSpeed = isRunning ? runSpeed : walkSpeed;

        // Handle jump input
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isJumping)
        {
            Jump();
        }

        // Create movement direction relative to camera
        if (cameraTransform != null)
        {
            // Get camera's forward and right directions (ignore Y component for ground movement)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Flatten the directions to the horizontal plane
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement direction relative to camera
            moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;
        }
        else
        {
            // Fallback to world-relative movement if no camera reference
            moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        }

        // Check if character is moving
        isMoving = moveDirection.magnitude > 0.1f;
    }

    void Jump()
    {
        if (rb != null && isGrounded)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCameraRotation()
    {
        if (!enableCameraRotation || cameraTransform == null) return;

        // Rotate camera horizontally when using A/D keys (only when not moving forward/backward)
        if (Mathf.Abs(verticalInput) < 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Rotate camera around the Y-axis based on horizontal input
            float cameraRotationAmount = horizontalInput * cameraRotationSpeed * Time.deltaTime;
            cameraTransform.RotateAround(transform.position, Vector3.up, cameraRotationAmount);

            // Make sure camera is still looking at the player
            Vector3 directionToPlayer = transform.position - cameraTransform.position;
            directionToPlayer.y = 0; // Keep camera at same height level
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    void HandleMovement()
    {
        if (rb == null) return;

        if (isMoving)
        {
            // Move the character
            Vector3 movement = moveDirection * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + movement);

            // Rotate character to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        // Set basic animation parameters
        animator.SetBool(idleAnimParam, !isMoving && isGrounded);
        animator.SetBool(walkAnimParam, isMoving && !isRunning && isGrounded);
        animator.SetBool(groundedAnimParam, isGrounded);
        
        // Set running parameter if it exists
        if (HasParameter(animator, runAnimParam))
        {
            animator.SetBool(runAnimParam, isRunning);
        }

        // Set jumping parameter if it exists
        if (HasParameter(animator, jumpAnimParam))
        {
            animator.SetBool(jumpAnimParam, isJumping || !isGrounded);
        }

        // Set speed parameter for blend trees if it exists
        if (HasParameter(animator, speedAnimParam))
        {
            float speedValue = 0f;
            if (isMoving && isGrounded)
            {
                speedValue = isRunning ? 2f : 1f; // 0 = idle, 1 = walk, 2 = run
            }
            animator.SetFloat(speedAnimParam, speedValue);
        }
    }

    // Utility method to check if animator has a specific parameter
    bool HasParameter(Animator animator, string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    // Public method to set camera reference (useful if camera changes)
    public void SetCameraReference(Transform newCameraTransform)
    {
        cameraTransform = newCameraTransform;
    }

    // Public method to toggle camera rotation feature
    public void SetCameraRotationEnabled(bool enabled)
    {
        enableCameraRotation = enabled;
    }

    // Public methods for external control
    public bool IsGrounded() { return isGrounded; }
    public bool IsJumping() { return isJumping; }
    public bool IsRunning() { return isRunning; }
    public bool IsMoving() { return isMoving; }

    // Draw ground check sphere in scene view for debugging
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}