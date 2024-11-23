using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float wallJumpForce = 7f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded = true;
    private bool isTouchingWall = false;
    private int jumpCount = 0;
    private int maxJumps = 2;

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
        HandleMouseLook(); // Rotate player based on mouse input
        CheckIfGrounded(); // Detect if the player is on the ground
        HandleJump(); // Manage all jump types
    }

    void FixedUpdate()
    {
        HandleMovement(); // Manage movement based on input
    }

    // Moves the player based on input and updates animations
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

    // Checks if the player is grounded and resets jumps
    void CheckIfGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0; // Allow jumping again
        }
    }

    // Handles jump logic: regular, double, and wall jumps
    void HandleJump()
    {
        if (jumpCount < maxJumps && Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump(Vector3.up * jumpForce);
            jumpCount++;
        }
        else if (isTouchingWall && Input.GetKeyDown(KeyCode.Space))
        {
            PerformWallJump();
        }
    }

    // Executes a jump in the specified direction
    void PerformJump(Vector3 jumpDirection)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(jumpDirection, ForceMode.Impulse);
        animator.SetTrigger("Jump");
        Debug.Log("Player Jumped");
    }

    // Executes a wall jump and resets jump count for air movement
    void PerformWallJump()
    {
        Vector3 wallJumpDirection = (Vector3.up + -transform.forward).normalized;
        PerformJump(wallJumpDirection * wallJumpForce);
        jumpCount = 1; // Allow one more jump in air
        Debug.Log("Player performed a wall jump");
    }

    // Detects collisions with ground and walls
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    // Resets wall state when leaving a wall
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    // Handles player rotation based on mouse input
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        Camera.main.transform.Rotate(Vector3.left * mouseY);
    }

    // Triggers a respawn when entering a respawn zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            RespawnPlayer();
        }
    }

    // Moves player to the respawn point
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
