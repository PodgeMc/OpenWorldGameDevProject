using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;     // Jumping force
    [SerializeField] private float climbingSpeed = 3f; // Speed at which the player climbs
    [SerializeField] private LayerMask climbableLayer; // Layer for climbable surfaces

    private Rigidbody rb;
    private Animator animator;

    private bool isClimbing = false;   // Track if player is climbing
    private bool canClimb = false;     // Track if player can climb
    private bool isGrounded = true;    // Track if player is grounded

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the player GameObject.");
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            HandleClimbing();
        }
        else
        {
            HandleMovement();
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleClimbInput();
        HandleJump();
    }

    // Handle normal walking/running movement
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;

        movement *= targetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        float speed = movement.magnitude;

        bool isWalking = speed > 0.1f && speed <= walkingSpeed;
        bool isRunning = speed > walkingSpeed;

        animator.SetBool("Walking", isWalking);
        animator.SetBool("Running", isRunning);
    }

    // Handle climbing movement
    void HandleClimbing()
    {
        float vertical = Input.GetAxis("Vertical");  // Vertical input for climbing up/down
        Vector3 climbMovement = new Vector3(0f, vertical * climbingSpeed * Time.fixedDeltaTime, 0f);

        // Use Rigidbody's MovePosition for smooth climbing
        rb.MovePosition(transform.position + climbMovement);

        // Update animation (if you have climbing animations)
        animator.SetBool("Climbing", true);

        // Exit climbing when the player presses "Jump" or the vertical input is 0
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitClimb();
        }
    }

    // Handle input for climbing
    void HandleClimbInput()
    {
        // Press 'E' to start climbing
        if (canClimb && Input.GetKeyDown(KeyCode.E))
        {
            EnterClimb();
        }
    }

    // Enter climbing state
    void EnterClimb()
    {
        isClimbing = true;
        rb.useGravity = false;  // Disable gravity when climbing
        rb.velocity = Vector3.zero;  // Stop any existing movement
        animator.SetBool("Climbing", true);  // Set climbing animation
    }

    // Exit climbing state
    void ExitClimb()
    {
        isClimbing = false;
        rb.useGravity = true;   // Re-enable gravity when done climbing
        animator.SetBool("Climbing", false);
    }

    // Handle jumping
    void HandleJump()
    {
        // Jump only when grounded and not climbing
        if (isGrounded && !isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetTrigger("Jump");
            Debug.Log("Player Jumping");
        }
    }

    // Detect when the player is grounded (e.g., when landing after a jump)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Handle mouse look (camera rotation)
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);

        float mouseY = Input.GetAxis("Mouse Y");
        Camera.main.transform.Rotate(Vector3.left * mouseY * rotationSpeed);
    }

    // Detect climbable surfaces using raycast or collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))  // Check if the object has the tag "Climbable"
        {
            canClimb = true;
        }
        else
        {
            canClimb = false;
        }

        if (other.CompareTag("Respawn"))
        {
            Debug.Log("Player entered respawn trigger.");
            RespawnPlayer(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            canClimb = false;
        }
    }

    void RespawnPlayer(GameObject player)
    {
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();

        if (respawnManager != null)
        {
            Debug.Log("Respawning player...");

            player.transform.position = respawnManager.respawnPoint.position;

            Debug.Log("Player respawned at: " + respawnManager.respawnPoint.position);
        }
        else
        {
            Debug.LogError("RespawnManager not found in the scene. Cannot respawn player.");
        }
    }
}
