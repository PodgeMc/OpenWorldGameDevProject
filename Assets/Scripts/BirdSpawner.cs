using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab; // The enemy prefab to spawn
    public Transform[] spawnPoints; // Array of spawn points for the enemies

    public void SpawnBird()
    {
        if (birdPrefab != null && spawnPoints.Length > 0)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the bird prefab at the chosen spawn point
            Instantiate(birdPrefab, spawnPoint.position, Quaternion.identity);

            Debug.Log("New bird spawned at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Bird prefab or spawn points are not set!");
        }
    }
}

