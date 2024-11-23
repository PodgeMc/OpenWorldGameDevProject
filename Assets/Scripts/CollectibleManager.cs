using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public int collectibleCount = 0;
    private bool isCarryingCollectible = false;
    private GameObject currentCollectible;
    public Text scoreText;
    public GameObject collectiblePrefab;
    public Transform[] spawnPoints;
    private int currentSpawnIndex = 0;

    private List<GameObject> collectiblePool = new List<GameObject>();

    public BirdSpawner birdSpawner;

    void Start()
    {
        // Initialize the collectible pool
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject collectible = Instantiate(collectiblePrefab, spawnPoint.position, Quaternion.identity);
            collectible.SetActive(false);
            collectiblePool.Add(collectible);
        }

        UpdateScoreText(); // Show initial score
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

    // Handles picking up a collectible
    void CollectItem(GameObject collectible)
    {
        collectible.SetActive(false);
        currentCollectible = collectible;
        collectibleCount++;
        isCarryingCollectible = true;

        UpdateScoreText();

        // Notify BirdSpawner if necessary
        if (birdSpawner != null)
        {
            birdSpawner.CheckAndSpawnBird(collectibleCount);
        }

        Debug.Log("Total Collectibles Collected: " + collectibleCount);
    }

    // Handles dropping the current collectible
    public void DropCollectible()
    {
        if (isCarryingCollectible && currentCollectible != null)
        {
            currentCollectible.SetActive(false);
            currentCollectible = null;
            isCarryingCollectible = false;

            ActivateNextCollectible();
        }
        else
        {
            Debug.Log("No collectible to drop.");
        }
    }

    // Activates the next available collectible from the pool
    private void ActivateNextCollectible()
    {
        foreach (GameObject collectible in collectiblePool)
        {
            if (!collectible.activeInHierarchy)
            {
                Transform spawnPoint = spawnPoints[currentSpawnIndex];
                collectible.transform.position = spawnPoint.position;
                collectible.SetActive(true);
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
                Debug.Log("Activated a collectible at: " + spawnPoint.position);
                return;
            }
        }

        Debug.LogWarning("No hidden collectibles left in the pool!");
    }

    // Updates the score display in the UI
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {collectibleCount}";
        }
        else
        {
            Debug.LogWarning("Score Text UI is missing!");
        }
    }

    // Returns whether the player is currently holding a collectible
    public bool IsCarryingCollectible()
    {
        return isCarryingCollectible;
    }
}
