using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public int damageAmount = 20;
    public float chaseSpeed = 8f;
    public float patrolSpeed = 2f;
    public float flyHeight = 5f;
    public float attackRange = 2f;
    public float followDistance = 20f;
    public bool idle = true;
    public bool fly = false;
    private bool hasDealtDamage = false;
    public Transform player;
    public AudioClip chaseSound;
    private SafeZone playerSafeZone;
    private Animator animator;
    private AudioSource audioSource;
    private PathFollower pathFollower;

    // Sets up required components for the bird
    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        audioSource = GetComponent<AudioSource>();
        playerSafeZone = player.GetComponent<SafeZone>();
        animator = GetComponent<Animator>();
    }

    // Handles bird behavior (chasing or patrolling) based on player position
    void Update()
    {
        if (player == null) return; // Do nothing if player is missing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within follow range and not safe, start chasing
        if (!playerSafeZone.isPlayerSafe && distanceToPlayer <= followDistance)
        {
            idle = false;
            fly = true;

            if (pathFollower != null) pathFollower.enabled = false; // Stop patrolling
            ChasePlayer();

            // Attack if close enough and hasn't attacked yet
            if (distanceToPlayer <= attackRange && !hasDealtDamage)
            {
                DealDamageToPlayer();
            }
        }
        else
        {
            // Switch back to patrolling when the player is out of range
            idle = true;
            fly = false;

            if (pathFollower != null) pathFollower.enabled = true; // Resume patrolling
            Patrol();
        }

        UpdateAnimations(); // Update animations to reflect the bird's state
    }

    // Moves the bird toward the player and maintains flight
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        // Move toward the player
        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Keep flying at a fixed height
        transform.position = new Vector3(transform.position.x, flyHeight, transform.position.z);

        // Smoothly rotate to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Play the chase sound if not already playing
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(chaseSound);
        }
    }

    // Handles idle patrolling when not chasing
    void Patrol()
    {
        // If no PathFollower is attached, rotate in place
        if (pathFollower == null)
        {
            transform.Rotate(0f, patrolSpeed * Time.deltaTime, 0f);
        }
    }

    // Deals damage to the player and resets the bird's state
    void DealDamageToPlayer()
    {
        hasDealtDamage = true; // Prevent multiple attacks

        // Attempt to find the player's health manager and apply damage
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damageAmount);
            Debug.Log($"Bird dealt {damageAmount} damage.");
        }

        ResetBird(); // Reset the bird's state after the attack
    }

    // Resets the bird to its default idle state
    void ResetBird()
    {
        hasDealtDamage = false; // Allow future attacks
        idle = true;
        fly = false;

        // Resume patrolling if a PathFollower is available
        if (pathFollower != null)
        {
            pathFollower.enabled = true;
        }
    }

    // Updates the bird's animations based on its current state
    void UpdateAnimations()
    {
        if (animator == null) return; // Do nothing if animator is missing

        animator.SetBool("Fly", fly);
        animator.SetBool("Idle", idle);
    }
}
