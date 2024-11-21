using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private int collectibleCount = 0;
    private bool isCarryingCollectible = false; // Tracks if the player is carrying a collectible
    private GameObject currentCollectible;     // Reference to the currently carried collectible

    void OnTriggerEnter(Collider other)
    {
        if (!isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Entered Trigger");
            CollectItem(other.gameObject);
        }
        else if (isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Already carrying an item. Can't pick up another one. Head to exit to start next run.");
        }
    }

    void CollectItem(GameObject collectible)
    {
        Debug.Log("Collecting Item");
        collectible.SetActive(false); // Deactivate the collectible
        currentCollectible = collectible; // Store a reference to the collected item
        collectibleCount++;
        isCarryingCollectible = true; // Set to true when the player picks up a collectible
        Debug.Log("Total Collectibles Collected: " + collectibleCount);
    }

    public void DropCollectible()
    {
        if (isCarryingCollectible && currentCollectible != null)
        {
            Debug.Log("Dropping Collectible");
            currentCollectible.SetActive(false); // Optionally deactivate the collectible (e.g., simulate depositing it)
            currentCollectible = null; // Clear the reference
            isCarryingCollectible = false; // Allow the player to pick up a new collectible
        }
        else
        {
            Debug.Log("No collectible to drop.");
        }
    }

    public bool IsCarryingCollectible()
    {
        return isCarryingCollectible; // Expose the carrying status to other scripts
    }
}
