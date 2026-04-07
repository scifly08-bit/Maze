using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour 
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float slideSpeed = 14f;
    [SerializeField] private float acceleration = 60f; // High acceleration for snappiness
    [SerializeField] private float friction = 50f;     // High friction to stop quickly

    [Header("Jumping & Gravity")]
    [SerializeField] private float jumpHeight = 3.2f;   // Higher burst
    [SerializeField] private float gravity = -95f;     // Massive gravity for fast rise/fall
    [SerializeField] private float airControl = 0.5f;
    [Tooltip("Maximum horizontal speed the player can reach while airborne. Lower = floatier/slower, Higher = more responsive mid-air movement.")]
    [SerializeField] private float airMoveSpeed = 6f;  // Max horizontal speed in the air
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float fallMultiplier = 1.25f; // Slight extra boost to fall

    [Header("Crouching & Sliding")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private float slideFriction = 10f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Camera cam;

    [Header("View Bobbing & Shake")]
    [SerializeField] private float bobFrequency = 14f;
    [SerializeField] private float bobHorizontalAmplitude = 0.06f;
    [SerializeField] private float bobVerticalAmplitude = 0.06f;
    [SerializeField] private float bobRotationSway = 1.5f; // Camera Roll (Z)
    [SerializeField] private float headBobSmoothing = 15f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckOffset = -0.1f;
    [SerializeField] private float groundCheckRadius = 0.35f;
    [SerializeField] private LayerMask groundMask = ~0;

    // Internal state
    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float xRotation = 0f;
    
    private bool isSprinting;
    private bool isCrouching;
    private bool isSliding;
    private float slideTimer;
    private float coyoteTimer;
    private bool isGrounded;

    // Head bobbing
    private float bobTimer;
    private Vector3 defaultCameraPos;
    private Vector3 cameraOffset;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cam != null) defaultCameraPos = cam.transform.localPosition;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Remove any Rigidbody that might fight with CharacterController
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.LogWarning("Rigidbody found on Player! CharacterController handles physics. Disabling Rigidbody.");
            rb.isKinematic = true; 
        }
    }

    void Update()
    {
        if (GameSettings.Pause) return;

        // Simple grounded check
        isGrounded = controller.isGrounded || CheckGroundedSphere();
        
        HandleInput();
        HandleMouseLook();
        HandleMovementVertical();
        HandleMovementHorizontal();
        HandleCrouchHeight();
        HandleCameraEffects();
        
        // Apply final combined movement
        Vector3 finalMove = (horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
        controller.Move(finalMove);
    }

    private void HandleInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.1f && !isCrouching && isGrounded;
        
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isSprinting && isGrounded && !isSliding) 
                StartSlide();
            else
                ToggleCrouch();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(Vector3.up * mouseX);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 95f);
    }

    void HandleMovementHorizontal()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        Vector3 inputDir = (transform.right * x + transform.forward * z);
        if (inputDir.magnitude > 1) inputDir.Normalize();

        float targetSpeed = walkSpeed;
        if (isSliding) targetSpeed = slideSpeed;
        else if (isSprinting) targetSpeed = sprintSpeed;
        else if (isCrouching) targetSpeed = crouchSpeed;

        // While airborne, cap horizontal speed to airMoveSpeed instead of the ground speed
        if (!isGrounded) targetSpeed = airMoveSpeed;

        float currentAccel = isGrounded ? acceleration : acceleration * airControl;
        float currentFriction = isGrounded ? friction : friction * 0.1f;

        if (inputDir.magnitude > 0.1f && !isSliding)
        {
            // MoveTowards is much MORE stable for snappy stopping and starting
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, inputDir * targetSpeed, currentAccel * Time.deltaTime);
        }
        else
        {
            float f = isSliding ? slideFriction : currentFriction;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, f * Time.deltaTime);
            
            if (isSliding && (horizontalVelocity.magnitude < walkSpeed || slideTimer <= 0))
                StopSlide();
        }

        if (isSliding) slideTimer -= Time.deltaTime;
    }

    void HandleMovementVertical()
    {
        // Gravity
        float gravityToApply = gravity;
        // Increase gravity significantly when falling for "snappy" landing
        if (verticalVelocity < 0) gravityToApply *= fallMultiplier;
        
        verticalVelocity += gravityToApply * Time.deltaTime;

        // Ground clamping
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -4f; // Even stronger downward pull to stick to ground
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && coyoteTimer > 0f)
        {
            // Calculate jump impulse from desired height
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteTimer = 0f;
            if (isSliding) StopSlide();
            if (isCrouching) ToggleCrouch();
        }
    }

    void HandleCrouchHeight()
    {
        float targetHeight = isCrouching || isSliding ? crouchHeight : standingHeight;
        
        // Smoothly adjust controller height
        float newHeight = Mathf.MoveTowards(controller.height, targetHeight, 10f * Time.deltaTime);
        controller.height = newHeight;
        controller.center = new Vector3(0, controller.height / 2f, 0);
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        horizontalVelocity = transform.forward * slideSpeed;
    }

    void StopSlide()
    {
        isSliding = false;
        // Keep crouching if key is held
        isCrouching = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftShift);
    }

    public void ApplyBounce(float force)
    {
        verticalVelocity = force;
        coyoteTimer = 0f;
        if (isSliding) StopSlide();
        if (isCrouching) ToggleCrouch();
    }

    public void SetMovementSpeeds(float walk, float sprint, float jump)
    {
        walkSpeed = walk;
        sprintSpeed = sprint;
        jumpHeight = jump;
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
    }

    void HandleCameraEffects()
    {
        float speed = horizontalVelocity.magnitude;
        float roll = 0;

        // View Bobbing & Sway
        if (isGrounded && speed > 0.5f && !isSliding)
        {
            // Faster bob frequency when sprinting
            float currentFreq = isSprinting ? bobFrequency * 1.5f : bobFrequency;
            bobTimer += Time.deltaTime * currentFreq;

            // Horizontal & Vertical bob (Sinusoidal)
            cameraOffset.x = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude;
            cameraOffset.y = Mathf.Sin(bobTimer) * bobVerticalAmplitude;

            if (isSprinting)
            {
                // Rotation Sway (Roll) - Adds that "running" lean
                roll = -Mathf.Sin(bobTimer * 0.5f) * (bobRotationSway * 1.5f);

                // Add natural noise for "shaky" run feel
                float noiseX = Mathf.PerlinNoise(Time.time * currentFreq, 0f) * 2f - 1f;
                float noiseY = Mathf.PerlinNoise(0f, Time.time * currentFreq) * 2f - 1f;
                cameraOffset.x += noiseX * 0.02f;
                cameraOffset.y += noiseY * 0.02f;
            }
        }
        else
        {
            bobTimer = 0;
            cameraOffset = Vector3.Lerp(cameraOffset, Vector3.zero, headBobSmoothing * Time.deltaTime);
            roll = Mathf.Lerp(0, 0, headBobSmoothing * Time.deltaTime);
        }

        // Combined camera position
        float targetCamHeight = (isCrouching || isSliding ? crouchHeight : standingHeight) - 0.2f;
        Vector3 targetLocalPos = new Vector3(0, targetCamHeight, 0) + cameraOffset;
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetLocalPos, headBobSmoothing * Time.deltaTime);

        // Apply rotation (Keep xRotation from mouse look, add roll)
        Quaternion targetRot = Quaternion.Euler(xRotation, 0, roll);
        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRot, headBobSmoothing * Time.deltaTime);
    }

    bool CheckGroundedSphere()
    {
        // Check slightly BELOW the feet
        Vector3 pos = transform.position + Vector3.up * 0.1f;
        return Physics.CheckSphere(pos, groundCheckRadius, groundMask);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if we hit a bouncy pad from above
        if (hit.normal.y > 0.7f && hit.gameObject.TryGetComponent(out BouncyPad pad))
        {
            ApplyBounce(pad.BounceForce);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + Vector3.up * 0.1f;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}
