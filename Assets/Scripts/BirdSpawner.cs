using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // The bird prefab to spawn
    public Transform[] spawnPoints; // Spawn points for birds
    public int poolSize = 20; // Number of birds to pre-instantiate

    private List<GameObject> birdPool = new List<GameObject>(); // The bird object pool
    private int lastScoreChecked = 0; // Tracks the last score milestone
    private int birdsToSpawn = 1; // Number of birds to spawn at each milestone

    void Start()
    {
        // Pre-instantiate the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bird = Instantiate(birdPrefab);
            bird.SetActive(false); // Deactivate initially
            birdPool.Add(bird); // Add to the pool
        }
    }

    public void CheckAndSpawnBird(int score)
    {
        Debug.Log($"CheckAndSpawnBird called with score: {score}");

        // Only spawn birds if the score is a multiple of 5 and hasn't already triggered
        if (score % 5 == 0 && score > lastScoreChecked)
        {
            lastScoreChecked = score; // Update the last checked score

            // Spawn multiple birds based on the current milestone
            for (int i = 0; i < birdsToSpawn; i++)
            {
                SpawnBird();
            }

            // Increment the number of birds to spawn for the next milestone
            birdsToSpawn++;

            Debug.Log($"Spawned {birdsToSpawn - 1} birds because the score reached a new milestone.");
        }
    }

    public void SpawnBird()
    {
        Debug.Log("Attempting to spawn a bird...");

        GameObject bird = GetPooledBird();

        if (bird != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            bird.transform.position = spawnPoint.position; // Set position
            bird.SetActive(true); // Reactivate the bird

            Debug.Log($"Bird spawned at: {spawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("No available birds in the pool!");
        }
    }

    private GameObject GetPooledBird()
    {
        foreach (GameObject bird in birdPool)
        {
            if (!bird.activeInHierarchy) // Find an inactive bird
            {
                return bird;
            }
        }

        Debug.LogWarning("No inactive birds found in the pool.");
        return null;
    }
}
