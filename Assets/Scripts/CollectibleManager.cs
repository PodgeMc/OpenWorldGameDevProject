using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private int collectibleCount = 0;
    private bool isCarryingCollectible = false;  // New variable to track if the player is carrying a collectible

    void OnTriggerEnter(Collider other)
    {
        if (!isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Entered Trigger");
            CollectItem(other.gameObject);
        }
        else if (isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Already carrying an item. Can't pick up another one. Head to exit to start next run. ");
        }
    }

    void CollectItem(GameObject collectible)
    {
        Debug.Log("Collecting Item");
        collectible.SetActive(false);
        collectibleCount++;
        isCarryingCollectible = true;  // Set to true when the player picks up a collectible
        Debug.Log("Total Collectibles Collected: " + collectibleCount);
        // You can perform additional logic or trigger events upon collecting an item
    }

    // You can call this method when you want the player to "drop" the currently held collectible
    public void DropCollectible()
    {
        isCarryingCollectible = false;
    }
}
