using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform respawnPoint;  // Drag and drop the RespawnPoint GameObject here in the Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the respawn trigger.");
            RespawnPlayer(other.gameObject);
        }
    }

    void RespawnPlayer(GameObject player)
    {
        Debug.Log("Respawning player...");
        // Reset player position to the respawn point
        player.transform.position = respawnPoint.position;
        Debug.Log("Player respawned at: " + respawnPoint.position);
        Debug.Log("RespawnManager script completed!");
    }
}
