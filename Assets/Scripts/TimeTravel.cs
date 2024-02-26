using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TimeTravel : MonoBehaviour
{
    public float Clock;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        Clock = 0f;


        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned to the PathFollower script.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Clock = Clock + (1/Time.deltaTime);

    }
}
