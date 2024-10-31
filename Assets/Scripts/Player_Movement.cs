using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;     // Jumping force
    [SerializeField] private float climbingSpeed = 3f; // Speed at which the player climbs
    [SerializeField] private LayerMask groundLayer;    // Layer for ground surfaces

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded = true;        // Track if player is grounded
    private bool canClimb = false;         // Track if player can climb
    private bool isClimbing = false;       // Track if player is currently climbing

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
        HandleJump();
        CheckIfGrounded(); // Continuously check if the player is grounded
    }

    // Handle normal walking/running movement
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;

        // Apply movement
        movement *= targetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        float speed = movement.magnitude;

        // Update animator parameters
        animator.SetBool("Walking", speed > 0.1f && speed <= walkingSpeed);
        animator.SetBool("Running", speed > walkingSpeed);

        if (speed > walkingSpeed)
        {
            canClimb = true; // Set climbing enabled when running
            Debug.Log("Player is running and can climb.");
        }
        else
        {
            canClimb = false; // Disable climbing when not running
        }
    }

    // Handle climbing movement
    void HandleClimbing()
    {
        float vertical = Input.GetAxis("Vertical");  // Vertical input for climbing up/down
        Vector3 climbMovement = new Vector3(0f, vertical * climbingSpeed * Time.fixedDeltaTime, 0f);

        // Move the player upwards while climbing
        rb.MovePosition(transform.position + climbMovement);
        animator.SetBool("Climbing", true);

        // Exit climbing if not pressing the forward key
        if (!Input.GetKey(KeyCode.W) || !isClimbing)
        {
            ExitClimb();
        }
    }

    // Handle jumping
    void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Set grounded to false until we land again
            animator.SetTrigger("Jump");
            Debug.Log("Player jumped.");
        }
    }

    // Detect when the player is grounded
    void OnCollisionEnter(Collision collision)
    {
        // Only set as grounded if the collision was with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player is grounded.");
        }

        // If colliding with a climbable wall while running, start climbing
        if (collision.gameObject.CompareTag("Climbable") && canClimb)
        {
            EnterClimb();
        }
    }

    // Check if the player is grounded using raycast
    void CheckIfGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer))
        {
            isGrounded = true;
            Debug.Log("Player detected as grounded by raycast.");
        }
        else
        {
            isGrounded = false;
        }
    }

    // Enter climbing state
    void EnterClimb()
    {
        isClimbing = true;
        rb.useGravity = false; // Disable gravity while climbing
        rb.velocity = Vector3.zero; // Stop any existing movement
        animator.SetBool("Climbing", true); // Set climbing animation
        Debug.Log("Player started climbing.");
    }

    // Exit climbing state
    void ExitClimb()
    {
        isClimbing = false;
        rb.useGravity = true; // Re-enable gravity when done climbing
        animator.SetBool("Climbing", false);
        Debug.Log("Player exited climbing.");
    }

    // Handle mouse look (camera rotation)
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);

        float mouseY = Input.GetAxis("Mouse Y");
        Camera.main.transform.Rotate(Vector3.left * mouseY * rotationSpeed);
        Debug.Log("Player is looking around.");
    }

    // Detect climbable surfaces using trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            Debug.Log("Player is near a climbable surface.");
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
            Debug.Log("Player exited climbable area.");
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
