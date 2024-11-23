using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    // Starts the game by loading the Test_Level scene
    public void PlayGame()
    {
        Debug.Log("Loading the game...");
        SceneManager.LoadScene("Test_Level"); // This is the main level for the game
    }

    // Goes back to the Main Menu scene
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to the main menu...");
        SceneManager.LoadScene("Main_Menu"); // This reloads the main menu
    }

    // Exits the game completely
    public void QuitGame()
    {
        Debug.Log("Quitting the game!");

#if UNITY_EDITOR
        // If I’m testing in the Unity Editor, this stops play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Closes the application if it’s a built version
        Application.Quit();
#endif
    }
}
