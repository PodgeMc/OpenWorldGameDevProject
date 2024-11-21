using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public bool isPlayerSafe = false; // Tracks if the player is in a safe zone
    public Transform dropOffPoint;   // Location to place the dropped object (assign in Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detect collision with the player
        {
            isPlayerSafe = true;
            Debug.Log("Player entered the safe zone!");

            // Check if the player has a collectible to drop off
            PlayerCollectible playerCollectible = other.GetComponent<PlayerCollectible>();
            if (playerCollectible != null && playerCollectible.carriedObject != null)
            {
                // Drop off the carried object
                DropOffCollectible(playerCollectible);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detect exiting the safe zone
        {
            isPlayerSafe = false;
            Debug.Log("Player exited the safe zone!");
        }
    }

    private void DropOffCollectible(PlayerCollectible playerCollectible)
    {
        GameObject carriedObject = playerCollectible.carriedObject;

        // Place the object at the drop-off point
        if (dropOffPoint != null)
        {
            carriedObject.transform.position = dropOffPoint.position;
        }

        // Optionally, disable or process the object (e.g., score calculation)
        carriedObject.SetActive(false);

        // Clear the player's carried object
        playerCollectible.carriedObject = null;

        Debug.Log("Object dropped off at the safe zone!");
    }
}
