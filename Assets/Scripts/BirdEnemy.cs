using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public Transform player; // The player
    public AudioClip eagleSound; //Eagle sound
    private AudioSource audioSource;
    private PathFollower pathFollower;
    public float speed = 5f;
    public float followDistance = 20f; // Maximum distance the bird will chase the player
    private bool isChasing = false; // Tracks bird chasing
    private SafeZone playerSafeZone; // Reference to the SafeZone

    void Start()
    {
        // Get the PathFollower component
        pathFollower = GetComponent<PathFollower>();
        if (pathFollower == null)
        {
            Debug.LogWarning("PathFollower script is not attached to the BirdEnemy GameObject!");
        }

        // Ensure player reference is assigned
        if (player == null)
        {
            Debug.LogError("Player reference is missing in BirdEnemy! Assign it in the Inspector.");
        }

        // Get the SafeZone component on the player
        playerSafeZone = player.GetComponent<SafeZone>();
        if (playerSafeZone == null)
        {
            Debug.LogError("SafeZone script is not attached to the player!");
        }

        // Set up AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on BirdEnemy! Please add one.");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is in a safe zone and within follow distance
        if (!playerSafeZone.isPlayerSafe && distanceToPlayer < followDistance)
        {
            if (!isChasing) // Trigger chase only if not already chasing
            {
                isChasing = true;
                PlayEagleSound(); // Play the eagle sound
                ChasePlayer();
            }
        }
        else
        {
            if (isChasing) // Trigger patrol only if chasing was active
            {
                isChasing = false;
                ResumePatrol();
            }
        }
    }

    void PlayEagleSound()
    {
        if (audioSource != null && eagleSound != null)
        {
            audioSource.PlayOneShot(eagleSound); // Play the sound once
            Debug.Log("Eagle sound played!");
        }
    }

    void ChasePlayer()
    {
        if (pathFollower != null) pathFollower.enabled = false; // Disable path-following while chasing

        Debug.Log("Bird is now chasing the player!");
    }

    void ResumePatrol()
    {
        if (pathFollower != null)
        {
            pathFollower.enabled = true; // Re-enable path-following
            Debug.Log("Bird has resumed patrolling.");
        }
        else
        {
            Debug.LogWarning("Bird cannot resume patrol because PathFollower is not attached!");
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            // Chase the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;

            // Smoothly rotate to face the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }
}
