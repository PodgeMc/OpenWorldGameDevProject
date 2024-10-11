using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 10f; // Adjust this speed as needed
    [SerializeField] private float rotationSpeed = 2f;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the player GameObject.");
        }
    }

    void FixedUpdate()
    {
        // Character Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Determine the target speed based on whether running or walking
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;

        movement *= targetSpeed * Time.fixedDeltaTime;

        // Use Rigidbody's MovePosition for more accurate movement
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        // Set "Walking" and "Running" parameters based on movement
        float speed = movement.magnitude;

        bool isWalking = speed > 0.1f && speed <= walkingSpeed;
        bool isRunning = speed > walkingSpeed;

        animator.SetBool("Walking", isWalking);
        animator.SetBool("Running", isRunning);

        // Rotate the character left and right
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);


    }

    void Update()
    {
        // Rotate the camera up and down
        float mouseY = Input.GetAxis("Mouse Y");
        Camera.main.transform.Rotate(Vector3.left * mouseY * rotationSpeed);

    }

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

            // Reset player position to the respawn point
            player.transform.position = respawnManager.respawnPoint.position;

            Debug.Log("Player respawned at: " + respawnManager.respawnPoint.position);
        }
        else
        {
            Debug.LogError("RespawnManager not found in the scene. Cannot respawn player.");
        }
    }
}
