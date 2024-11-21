using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public bool isPlayerSafe = false; // Tracks if the player is in any safe zone
    public Transform dropOffPoint;   // Location to place the dropped object (assign in Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detect collision with the player
        {
            isPlayerSafe = true;
            Debug.Log("Player entered the safe zone!");

            // Check if the player is carrying a collectible
            CollectibleManager collectibleManager = other.GetComponent<CollectibleManager>();
            if (collectibleManager != null && collectibleManager.IsCarryingCollectible())
            {
                // Drop off the collectible
                collectibleManager.DropCollectible();
                Debug.Log("Collectible dropped off at the safe zone!");
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
}
