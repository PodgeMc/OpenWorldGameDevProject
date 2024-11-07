using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Set how fast the player walks
    [SerializeField] private float walkingSpeed = 5f;
    // Set how fast the player runs
    [SerializeField] private float runningSpeed = 10f;
    // Set how fast the player can turn
    [SerializeField] private float rotationSpeed = 2f;
    // Set how high the player can jump
    [SerializeField] private float jumpForce = 5f;
    // Set which objects are considered the "ground" 
    [SerializeField] private LayerMask groundLayer;

    // Store the player's Rigidbody (for movement)
    private Rigidbody rb;
    // Store the player's Animator (for animations)
    private Animator animator;

    // Check if the player is on the ground
    private bool isGrounded = true;

    void Start()
    {
        // Get the Rigidbody on the player
        rb = GetComponent<Rigidbody>();
        // Make sure the player doesn’t tip over
        rb.freezeRotation = true;

        // Hide and lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the Animator for the player
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            // Print an error if no Animator is found
            Debug.LogError("Animator component not found on the player GameObject.");
        }
    }

    void FixedUpdate()
    {
        // Handle the player’s movement
        HandleMovement();
    }

    void Update()
    {
        // Handle where the player looks (camera rotation)
        HandleMouseLook();
        // Check if the player wants to jump
        Jump();
        // Check if the player is touching the ground
        CheckIfGrounded();
    }

    // Make the player walk or run
    void HandleMovement()
    {
        // Get the horizontal and vertical movement (from arrow keys or WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Set the movement direction
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Choose running or walking speed
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;

        // Move the player based on speed and direction
        movement *= targetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        // Get the speed to update animations
        float speed = movement.magnitude;

        // Tell the animator when the player is walking or running
        animator.SetBool("Walking", speed > 0.1f && speed <= walkingSpeed);
        animator.SetBool("Running", speed > walkingSpeed);
    }

    // Make the player jump
    void Jump()
    {
        // Only jump if the player is on the ground and pressing Space
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Apply a force upward
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // The player is no longer on the ground
            isGrounded = false;
            // Play the jump animation
            animator.SetTrigger("Jump");
            Debug.Log("Player Jumped");
        }
    }

    // Check if the player is on the ground
    void CheckIfGrounded()
    {
        // Cast a ray downwards to see if the player is on the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    // Handle player looking around with the mouse
    void HandleMouseLook()
    {
        // Rotate player horizontally based on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically based on mouse movement
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        Camera.main.transform.Rotate(Vector3.left * mouseY);
    }

    // Check for collisions when the player hits something
    void OnCollisionEnter(Collision collision)
    {
        // Check if the player landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Set the player as grounded (on the ground)
            isGrounded = true;
            Debug.Log("Player landed on ground.");
        }

        // Check if the player hit a wall and is pressing W to climb
        if (collision.gameObject.CompareTag("Climbable") && Input.GetKey(KeyCode.W))
        {
            // Apply an upward force to climb the wall
            Vector3 wallClimbForce = Vector3.up * jumpForce;
            rb.AddForce(wallClimbForce, ForceMode.Impulse);
            Debug.Log("Player is climbing the wall.");
        }
    }

    // Keep checking if the player is still on the ground
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Check if the player enters a trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            // Respawn the player if they enter a respawn zone
            Debug.Log("Player entered respawn trigger.");
            RespawnPlayer(gameObject);
        }
    }

    // Move the player back to a respawn point
    void RespawnPlayer(GameObject player)
    {
        // Find the respawn manager in the scene
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();

        if (respawnManager != null)
        {
            // Print a message to show the player is respawning
            Debug.Log("Respawning player...");

            // Move the player to the respawn point location
            player.transform.position = respawnManager.respawnPoint.position;

            // Show the new position in the console
            Debug.Log("Player respawned at: " + respawnManager.respawnPoint.position);
        }
        else
        {
            // Print an error if no respawn manager is found
            Debug.LogError("RespawnManager not found in the scene. Cannot respawn player.");
        }
    }
}
