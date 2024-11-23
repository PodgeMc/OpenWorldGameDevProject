using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public int collectibleCount = 0; // Total score
    private bool isCarryingCollectible = false; // If I’m holding a collectible
    private GameObject currentCollectible; // The collectible I’m carrying
    public Text scoreText; // The score display
    public GameObject collectiblePrefab; // The collectible prefab
    public Transform[] spawnPoints; // Where collectibles spawn
    private int currentSpawnIndex = 0; // Tracks the next spawn point

    private List<GameObject> collectiblePool = new List<GameObject>(); // Pool of collectibles

    public BirdSpawner birdSpawner; // Tells the BirdSpawner when to spawn a bird

    void Start()
    {
        // Create collectibles and add them to the pool
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject collectible = Instantiate(collectiblePrefab, spawnPoint.position, Quaternion.identity);
            collectible.SetActive(false); // Hide them at the start
            collectiblePool.Add(collectible);
        }

        UpdateScoreText(); // Show the initial score
        ActivateNextCollectible(); // Activate the first collectible
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            CollectItem(other.gameObject); // Pick up the collectible
        }
        else if (isCarryingCollectible && other.CompareTag("Collectibles"))
        {
            Debug.Log("Already holding a collectible. Drop it off first.");
        }
    }

    void CollectItem(GameObject collectible)
    {
        collectible.SetActive(false); // Hide the collected item
        currentCollectible = collectible; // Save the collected item
        collectibleCount++; // Increase the score
        isCarryingCollectible = true; // Mark that I’m holding something

        UpdateScoreText();

        // Tell the BirdSpawner if we need a new bird
        if (birdSpawner != null)
        {
            birdSpawner.CheckAndSpawnBird(collectibleCount);
        }

        Debug.Log("Total Collectibles Collected: " + collectibleCount);
    }

    public void DropCollectible()
    {
        if (isCarryingCollectible && currentCollectible != null)
        {
            currentCollectible.SetActive(false); // Hide the dropped collectible
            currentCollectible = null; // Forget the dropped item
            isCarryingCollectible = false; // Ready to pick up another

            ActivateNextCollectible(); // Activate a new collectible
        }
        else
        {
            Debug.Log("No collectible to drop.");
        }
    }

    private void ActivateNextCollectible()
    {
        foreach (GameObject collectible in collectiblePool)
        {
            if (!collectible.activeInHierarchy) // Find a hidden collectible
            {
                Transform spawnPoint = spawnPoints[currentSpawnIndex];
                collectible.transform.position = spawnPoint.position; // Move it to the next spawn point
                collectible.SetActive(true); // Show it
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length; // Cycle to the next spawn point
                Debug.Log("Activated a collectible at: " + spawnPoint.position);
                return;
            }
        }

        Debug.LogWarning("No hidden collectibles left in the pool!");
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {collectibleCount}"; // Update the score display
        }
        else
        {
            Debug.LogWarning("Score Text UI is missing!");
        }
    }

    public bool IsCarryingCollectible()
    {
        return isCarryingCollectible; // True if holding something
    }
}
