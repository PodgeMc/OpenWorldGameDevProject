using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public int damageAmount = 20; // Damage dealt to the player
    public float chaseSpeed = 5f; // Speed while chasing the player
    public float attackRange = 2f; // Range to deal damage to the player
    public float followDistance = 20f; // Maximum distance to start chasing
    private bool isChasing = false; // Tracks if the bird is chasing the player
    private bool hasDealtDamage = false; // Prevents multiple damage events

    public Transform player; // Reference to the player
    public BirdSpawner spawner; // Reference to the spawner script
    private PathFollower pathFollower; // Reference to the PathFollower script

    public AudioClip chaseSound; // Audio clip for when the bird starts chasing
    private AudioSource audioSource; // AudioSource component for playing sounds

    private SafeZone playerSafeZone; // Reference to the player's SafeZone status

    void Start()
    {
        // Get the PathFollower component
        pathFollower = GetComponent<PathFollower>();
        if (pathFollower == null)
        {
            Debug.LogWarning("PathFollower script is not attached to BirdEnemy.");
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing on BirdEnemy.");
        }

        // Get the SafeZone component from the player
        playerSafeZone = player.GetComponent<SafeZone>();
        if (playerSafeZone == null)
        {
            Debug.LogWarning("SafeZone script is not attached to the player.");
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing on BirdEnemy!");
            return;
        }

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is in a safe zone or within follow distance
        if (!playerSafeZone.isPlayerSafe && distanceToPlayer <= followDistance)
        {
            if (!isChasing) // Start chasing only if not already chasing
            {
                isChasing = true;

                if (pathFollower != null)
                {
                    pathFollower.enabled = false; // Disable patrolling
                }

                PlayChaseSound(); // Play chase sound
            }

            ChasePlayer();

            // Check if the bird is close enough to attack
            if (distanceToPlayer <= attackRange && !hasDealtDamage)
            {
                DealDamageToPlayer();
            }
        }
        else
        {
            // Resume patrolling
            isChasing = false;

            if (pathFollower != null)
            {
                pathFollower.enabled = true; // Enable patrolling
            }
        }
    }

    void ChasePlayer()
    {
        // Calculate the direction toward the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Move the bird toward the player at chase speed
        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Smoothly rotate the bird to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        Debug.Log("Bird is chasing the player...");
    }

    void DealDamageToPlayer()
    {
        hasDealtDamage = true; // Mark damage as dealt
        PlayerManager playerManager = player.GetComponent<PlayerManager>();

        if (playerManager != null)
        {
            playerManager.TakeDamage(damageAmount); // Inflict damage
            Debug.Log($"Bird dealt {damageAmount} damage to the player.");
        }

        // Notify the spawner and deactivate the bird
        NotifySpawnerAndDeactivate();
    }

    void NotifySpawnerAndDeactivate()
    {
        if (spawner != null)
        {
            Debug.Log("Notifying spawner to spawn a new bird...");
            spawner.SpawnBird(); // Trigger the spawner to create a new bird
        }
        else
        {
            Debug.LogWarning("Spawner reference is missing!");
        }

        Debug.Log("Deactivating bird...");
        hasDealtDamage = false; // Reset damage state for reuse
        isChasing = false; // Reset chasing state
        gameObject.SetActive(false); // Deactivate this bird
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDealtDamage)
        {
            DealDamageToPlayer();
        }
    }

    private void OnDisable()
    {
        // Reset bird state when deactivated if necessary
        if (pathFollower != null)
        {
            pathFollower.enabled = true; // Ensure patrolling is enabled for reuse
        }
    }

    private void PlayChaseSound()
    {
        if (audioSource != null && chaseSound != null)
        {
            audioSource.PlayOneShot(chaseSound); // Play the chase sound
            Debug.Log("Chase sound played!");
        }
        else
        {
            Debug.LogWarning("AudioSource or chaseSound is missing.");
        }
    }
}
