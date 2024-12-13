using UnityEngine;using System.Collections;
using UnityEngine.SceneManagement;

public class PauseResumeHandler : MonoBehaviour
{
    public GameObject resumePanel;  // The resume panel UI
    public GameObject mainPanel;   // The main game UI panel
    public GameObject panels;      // The Panels GameObject (UI)
    public GameObject gameAudio; 
    public Camera mainCamera;      // Main camera for gameplay
    private bool isPaused = false; // Track the pause state
    public AudioSource backgroundAudioSource;
    private string selectedCharacter;
    

    void Start()
    {

        isPaused = false;
selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", ""); 
        // Log initial states
        Debug.Log($"PauseResumeHandler initialized. Panels hidden. isPaused: {isPaused}");
    }

    void Update()
    {
        // Listen for the ESC key press to toggle pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed. Current isPaused state: " + isPaused);

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void Quit()
    {
         if (resumePanel != null)
        {
            resumePanel.SetActive(false);
            Debug.Log("Resume panel activated.");
        }
        if (panels != null)
        {
            panels.SetActive(true);
            Debug.Log("Panels activated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            Debug.Log("Main panel deactivated.");
        }

    }

    void PauseGame()
    {
        Debug.Log("Pausing the game...");

        // Set time scale to 0 to pause gameplay
        Time.timeScale = 0;
        isPaused = true;

        // Activate pause-related UI
        if (resumePanel != null)
        {
            resumePanel.SetActive(true);
            Debug.Log("Resume panel activated.");
        }
        if (panels != null)
        {
            panels.SetActive(true);
            Debug.Log("Panels activated.");
        }
        if (gameAudio != null)
        {
            gameAudio.SetActive(false);
            Debug.Log("gameAudio activated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
            Debug.Log("Main panel deactivated.");
        }
        backgroundAudioSource.Pause();
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming the game...");

        // Set time scale back to 1 to resume gameplay
       
        isPaused = false;

        // Deactivate pause-related UI
        if (resumePanel != null)
        {
            resumePanel.SetActive(false);
            Debug.Log("Resume panel deactivated.");
        }
        if (panels != null)
        {
            panels.SetActive(false);
            Debug.Log("Panels deactivated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            Debug.Log("Main panel activated.");
        }
         if (gameAudio != null)
        {
            gameAudio.SetActive(true);
            Debug.Log("gameAudio activated.");
        }
        backgroundAudioSource.UnPause();
    }
    public void RestartLevel()
    {
        // Reload the current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);

        // After scene reload, we handle the character and game state.
        Invoke("AfterSceneLoad", 0.1f); // Small delay to allow scene load to complete
    }

    private void AfterSceneLoad()
    {
        // Reset panels and activate the character after scene reload
        if (panels != null)
        {
            panels.SetActive(false);  // Deactivate Panels UI
            Debug.Log("Panels deactivated.");
        }

        // Activate the selected character based on PlayerPrefs
        ActivateCharacterOnRestart();
    }

    private void ActivateCharacterOnRestart()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogError("No character selected!");
            return;
        }

        // Deactivate all characters initially
        GameObject sorcererPlayer = GameObject.Find("sorcerer");
        GameObject barbarianPlayer = GameObject.Find("barbarian");
        GameObject roguePlayer = GameObject.Find("rouge");

        if (sorcererPlayer != null) sorcererPlayer.SetActive(false);
        if (barbarianPlayer != null) barbarianPlayer.SetActive(false);
        if (roguePlayer != null) roguePlayer.SetActive(false);

        // Reactivate the selected character
        switch (selectedCharacter.ToLower())
        {
            case "sorcerer":
                if (sorcererPlayer != null) sorcererPlayer.SetActive(true);
                break;
            case "barbarian":
                if (barbarianPlayer != null) barbarianPlayer.SetActive(true);
                break;
            case "rouge":
                if (roguePlayer != null) roguePlayer.SetActive(true);
                break;
        }
    }

    

}
