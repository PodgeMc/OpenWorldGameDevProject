using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public bool isPlayerSafe = false; // True if the player is in the safe zone

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerSafe = true;
            Debug.Log("Player entered the safe zone!");

            // Check if the player is carrying a collectible and drop it off
            CollectibleManager collectibleManager = other.GetComponent<CollectibleManager>();
            if (collectibleManager != null && collectibleManager.IsCarryingCollectible())
            {
                collectibleManager.DropCollectible();
                Debug.Log("Collectible dropped off at the safe zone!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerSafe = false;
            Debug.Log("Player exited the safe zone!");
        }
    }
}
