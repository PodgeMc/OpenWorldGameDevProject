using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // The bird prefab to spawn
    public Transform[] spawnPoints; // Spawn points for birds
    public int poolSize = 10; // Number of birds to pre-instantiate

    private List<GameObject> birdPool = new List<GameObject>(); // The bird object pool

    void Start()
    {
        // Pre-instantiate the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bird = Instantiate(birdPrefab);
            bird.SetActive(false); // Deactivate initially
            birdPool.Add(bird); // Add to the pool
        }

        // Spawn one bird at the start of the game
        SpawnBird();
    }

    public void CheckAndSpawnBird(int score)
    {
        Debug.Log($"CheckAndSpawnBird called with score: {score}");

        // Spawn a bird when the score is a multiple of 5
        if (score % 5 == 0)
        {
            Debug.Log("Spawning a bird because the score is a multiple of 5.");
            SpawnBird();
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
