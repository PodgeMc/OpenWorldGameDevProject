using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab;
    public Transform[] spawnPoints;
    public int poolSize = 20;

    private List<GameObject> birdPool = new List<GameObject>();
    private int lastScoreChecked = 0; // Prevents duplicate spawns for the same milestone
    private int birdsToSpawn = 1; // Number of birds to spawn at each milestone

    void Start()
    {
        // Create the bird pool and deactivate all birds
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bird = Instantiate(birdPrefab);
            bird.SetActive(false);
            birdPool.Add(bird);
        }
    }

    // Spawns birds when the score reaches a new milestone
    public void CheckAndSpawnBird(int score)
    {
        Debug.Log($"CheckAndSpawnBird called with score: {score}");

        if (score % 5 == 0 && score > lastScoreChecked) // Trigger only on new milestones
        {
            lastScoreChecked = score;

            for (int i = 0; i < birdsToSpawn; i++) // Spawn multiple birds
            {
                SpawnBird();
            }

            birdsToSpawn++; // Increase birds spawned for next milestone
            Debug.Log($"Spawned {birdsToSpawn - 1} birds for milestone score: {score}");
        }
    }

    // Spawns a single bird from the pool
    public void SpawnBird()
    {
        GameObject bird = GetPooledBird();

        if (bird != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            bird.transform.position = spawnPoint.position;
            bird.SetActive(true);
            Debug.Log($"Bird spawned at: {spawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("No available birds in the pool!");
        }
    }

    // Finds an inactive bird in the pool
    private GameObject GetPooledBird()
    {
        foreach (GameObject bird in birdPool)
        {
            if (!bird.activeInHierarchy)
            {
                return bird;
            }
        }

        Debug.LogWarning("No inactive birds available.");
        return null;
    }
}
