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

    void Start()
    {
        // Get required components
        pathFollower = GetComponent<PathFollower>();
        audioSource = GetComponent<AudioSource>();
        playerSafeZone = player.GetComponent<SafeZone>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference missing!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Start chasing if the player is within range and not in a safe zone
        if (!playerSafeZone.isPlayerSafe && distanceToPlayer <= followDistance)
        {
            idle = false;
            fly = true;
            if (pathFollower != null) pathFollower.enabled = false;
            ChasePlayer();

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
            if (pathFollower != null) pathFollower.enabled = true;
            Patrol();
        }

        UpdateAnimations();
    }

    void ChasePlayer()
    {
        // Move toward the player and maintain flying height
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, flyHeight, transform.position.z);

        // Rotate to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Play chase sound
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(chaseSound);
        }
    }

    void Patrol()
    {
        // Simple patrolling behavior
        if (pathFollower == null)
        {
            transform.Rotate(0f, patrolSpeed * Time.deltaTime, 0f);
        }
    }

    void DealDamageToPlayer()
    {
        hasDealtDamage = true;

        // Damage the player
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damageAmount);
            Debug.Log($"Bird dealt {damageAmount} damage.");
        }

        ResetBird();
    }

    void ResetBird()
    {
        // Reset bird to idle state
        hasDealtDamage = false;
        idle = true;
        fly = false;
        if (pathFollower != null)
        {
            pathFollower.enabled = true;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Update animation states
        animator.SetBool("Fly", fly);
        animator.SetBool("Idle", idle);
    }
}
