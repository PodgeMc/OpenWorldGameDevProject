using UnityEngine;
using UnityEngine.UI; // Required for working with UI components

public class CollectibleManager : MonoBehaviour
{
    private int collectibleCount = 0; // Number of collectibles picked up
    private bool isCarryingCollectible = false; // Tracks if the player is carrying a collectible
    private GameObject currentCollectible; // Reference to the currently carried collectible

    public Text scoreText; // Reference to the UI Text component for displaying the score

    void Start()
    {
        // Initialize the score text at the start of the game
        UpdateScoreText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Entered Trigger");
            CollectItem(other.gameObject);
        }
        else if (isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Already carrying an item. Can't pick up another one. Head to a safe zone to drop it off.");
        }
    }

    void CollectItem(GameObject collectible)
    {
        Debug.Log("Collecting Item");
        collectible.SetActive(false); // Deactivate the collectible
        currentCollectible = collectible; // Store a reference to the collected item
        collectibleCount++;
        isCarryingCollectible = true; // Set to true when the player picks up a collectible

        // Update the score UI
        UpdateScoreText();

        Debug.Log("Total Collectibles Collected: " + collectibleCount);
    }

    public void DropCollectible()
    {
        if (isCarryingCollectible && currentCollectible != null)
        {
            Debug.Log("Dropping Collectible");
            currentCollectible.SetActive(false); // Optionally deactivate the collectible
            currentCollectible = null; // Clear the reference
            isCarryingCollectible = false; // Allow the player to pick up a new collectible

            // Optionally update the score or trigger events here
        }
        else
        {
            Debug.Log("No collectible to drop.");
        }
    }

    // Method to check if the player is carrying a collectible
    public bool IsCarryingCollectible()
    {
        return isCarryingCollectible;
    }

    // Updates the score text UI
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + collectibleCount;
        }
        else
        {
            Debug.LogWarning("Score Text UI is not assigned in the Inspector!");
        }
    }
}
