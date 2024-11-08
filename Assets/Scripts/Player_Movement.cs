using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Basic movement and jump settings
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;

    // Wall jump settings
    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 7f;
    [SerializeField] private LayerMask wallLayer;  // Defines "walls" for jumping

    // Ground detection settings (including roofs and tops of walls)
    [SerializeField] private LayerMask groundLayer;  // Treat roof and wall tops as ground

    private Rigidbody rb;  // For player movement
    private Animator animator;  // For player animations

    private bool isGrounded = true;  // True when on a surface
    private bool isTouchingWall = false;  // True if touching a wall
    private int jumpCount = 0;  // Tracks number of jumps
    private int maxJumps = 2;  // Max jumps allowed (1 regular + 1 double jump)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();  // Handle looking around with the mouse
        CheckIfGrounded();  // Check if on a flat surface
        HandleJump();  // Handle jumping (ground, double, and wall jumps)
    }

    void FixedUpdate()
    {
        HandleMovement();  // Move the player based on input
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;

        movement *= targetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        float speed = movement.magnitude;
        animator.SetBool("Walking", speed > 0.1f && speed <= walkingSpeed);
        animator.SetBool("Running", speed > walkingSpeed);
    }

    // Check if the player is on a flat surface like the ground, roof, or wall top
    void CheckIfGrounded()
    {
        // Raycast down to see if player is on ground or any flat surface in the groundLayer
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        // Reset jumps when on a flat surface
        if (isGrounded)
        {
            jumpCount = 0;  // Reset jump count
        }
    }

    // Handles jumping (ground, double, and wall jump)
    void HandleJump()
    {
        // Regular jump or double jump
        if (jumpCount < maxJumps && Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump(Vector3.up * jumpForce);  // Jump straight up
            jumpCount++;  // Increment jump count after each jump
        }
        // Wall jump when touching a wall
        else if (isTouchingWall && Input.GetKeyDown(KeyCode.Space))
        {
            PerformWallJump();
        }
    }

    // General jump function
    void PerformJump(Vector3 jumpDirection)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);  // Reset y-velocity for consistent jump height
        rb.AddForce(jumpDirection, ForceMode.Impulse);
        animator.SetTrigger("Jump");
        Debug.Log("Player Jumped");
    }

    // Wall jump function that pushes the player away from the wall
    void PerformWallJump()
    {
        Vector3 wallJumpDirection = (Vector3.up + -transform.forward).normalized;
        PerformJump(wallJumpDirection * wallJumpForce);
        Debug.Log("Player performed a wall jump");

        // Reset jump count so player can double jump after wall jump
        jumpCount = 1;  // Set to 1 so they can perform one more jump in air
    }

    // Detects collisions with walls for wall-jumping
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // Set grounded when player lands
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;  // Player is touching a wall
        }
    }

    // Reset wall-touch status when leaving a wall
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;  // Player is no longer touching a wall
        }
    }

    // Handle player looking around with the mouse
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        Camera.main.transform.Rotate(Vector3.left * mouseY);
    }

    // Trigger-based respawn functionality
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            RespawnPlayer();
        }
    }

    // Respawn player at a specified point
    void RespawnPlayer()
    {
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
        if (respawnManager != null)
        {
            Debug.Log("Respawning player...");
            transform.position = respawnManager.respawnPoint.position;
            Debug.Log("Player respawned at: " + respawnManager.respawnPoint.position);
        }
        else
        {
            Debug.LogError("RespawnManager not found in the scene.");
        }
    }
}
