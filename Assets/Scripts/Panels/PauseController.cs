using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu;
     public GameObject MainMenu;


    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0; // Freeze game
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1; // Resume game
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        MainMenu.SetActive(true);
        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene
    }
}
