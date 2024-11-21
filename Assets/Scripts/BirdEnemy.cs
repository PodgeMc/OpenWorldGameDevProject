using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public int damageAmount = 20; // Damage dealt to the player
    private bool hasDealtDamage = false; // Prevent multiple damage events

    public BirdSpawner spawner; // Reference to the spawner script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDealtDamage)
        {
            hasDealtDamage = true; // Ensure damage is dealt only once
            PlayerManager playerManager = other.GetComponent<PlayerManager>();

            if (playerManager != null)
            {
                playerManager.TakeDamage(damageAmount); // Deal damage to the player
            }

            NotifySpawnerAndDestroy(); // Notify the spawner and destroy the enemy
        }
    }

    void NotifySpawnerAndDestroy()
    {
        if (spawner != null)
        {
            Debug.Log("Notifying spawner to spawn a new bird...");
            spawner.SpawnBird(); // Spawn a new bird
        }
        else
        {
            Debug.LogWarning("Spawner reference is missing!");
        }

        Debug.Log("Destroying bird...");
        Destroy(gameObject); // Destroy the current bird
    }
}
