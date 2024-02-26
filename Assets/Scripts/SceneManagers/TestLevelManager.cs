using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestLevelManager : MonoBehaviour
{
    [System.Serializable]
    public class DropOption // Serializable class for drop options
    {
        public GameObject playerCopy; // The object to drop
    }

    public DropOption[] playerCopy;

    public bool spotted;
    public bool playerHiding;
    public GameObject SpawnLocation;
    public GameObject Player;
    public float timer;
    public int itemsCollected;
    public int attemptNumber;
    public int runWhoSpotted;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        playerHiding = false;

        if(SceneManager.GetActiveScene().name == "TestLevel")
        {
            attemptNumber = 1;
            itemsCollected = 0;
        }

        else if(SceneManager.GetActiveScene().name == "Level_1")
        {
        }

        else if (SceneManager.GetActiveScene().name == "Level_2")
        {
        }

        else
        {
            Debug.Log("No valid scene loaded");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + 1;

        if (SceneManager.GetActiveScene().name == "TestLevel")
        {

        }

        else if (SceneManager.GetActiveScene().name == "Level_1")
        {

        }

        else if (SceneManager.GetActiveScene().name == "Level_2")
        {

        }

        else
        {
            Debug.Log("No valid scene loaded");
        }
    }
}
