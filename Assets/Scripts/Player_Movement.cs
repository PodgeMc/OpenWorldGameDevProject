using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;     // Jumping force
    [SerializeField] private LayerMask groundLayer;    // Layer for ground surfaces

    private Rigidbody rb;
    private Animator animator;

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
        HandleMovement();
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

        movement *= targetSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        float speed = movement.magnitude;

        // Update animator parameters
        animator.SetBool("Walking", speed > 0.1f && speed <= walkingSpeed);
        animator.SetBool("Running", speed > walkingSpeed);
    }

    // Handle jumping
    void HandleJump()
    {
        // Jump only when grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Set grounded to false until we land again
            animator.SetTrigger("Jump");
            Debug.Log("Player Jumped");
        }
    }

    // Detect when the player is grounded using raycast
    void CheckIfGrounded()
    {
        // Check if the player's vertical velocity is near zero, indicating they're grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        if (isGrounded)
        {
            Debug.Log("Player is grounded.");
        }
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

    // Detect collisions for ground check
    void OnCollisionEnter(Collision collision)
    {
        // Only set as grounded if the collision was with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player landed on ground.");
        }
    }

    // Detect triggers
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            Debug.Log("Player entered respawn trigger.");
            RespawnPlayer(gameObject);
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
